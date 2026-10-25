using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FootballTvPlanner.Server.Common.Email;

public class EmailService(
    IOptions<EmailSettings> emailSettings,
    EmailTemplateRenderer templateRenderer,
    EmailRateLimiter emailRateLimiter,
    EmailMetrics metrics,
    ILogger<EmailService> logger
) : IEmailService
{
    public async Task SendTemplateEmailAsync(
        string toAddress,
        string subject,
        string template,
        object model,
        CancellationToken cancellationToken = default
    )
    {
        if (!emailRateLimiter.TryAcquire(toAddress))
        {
            metrics.RecordSent(template, outcome: "throttled");

            logger.LogWarning(
                "Skipped email with template {Template} as the recipient has hit the limit",
                template
            );

            return;
        }

        try
        {
            var settings = emailSettings.Value;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = await templateRenderer.RenderAsync(template, model),
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                settings.SmtpServer,
                settings.SmtpPort,
                settings.SmtpSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken
            );

            if (
                !string.IsNullOrEmpty(settings.SmtpUserName)
                && !string.IsNullOrEmpty(settings.SmtpPassword)
            )
            {
                await client.AuthenticateAsync(
                    settings.SmtpUserName,
                    settings.SmtpPassword,
                    cancellationToken
                );
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            metrics.RecordSent(template, outcome: "success");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            metrics.RecordSent(template, outcome: "failure");
            Activity.Current?.AddException(ex).SetStatus(ActivityStatusCode.Error);
            logger.LogError(ex, "Failed to send email with template {Template}", template);
        }
    }
}
