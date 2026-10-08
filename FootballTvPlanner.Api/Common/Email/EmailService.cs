using System.Reflection;
using FluentEmail.Core;

namespace FootballTvPlanner.Api.Common.Email;

public class EmailService(IFluentEmail fluentEmail, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendTemplateEmailAsync(
        string toAddress,
        string subject,
        string template,
        object model,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var sendResponse = await fluentEmail
                .To(toAddress)
                .Subject(subject)
                .UsingTemplateFromEmbedded(template, model, Assembly.GetExecutingAssembly())
                .SendAsync(cancellationToken);

            if (!sendResponse.Successful)
            {
                logger.LogError(
                    "Failed to send email with template {Template}: {Errors}",
                    template,
                    string.Join("; ", sendResponse.ErrorMessages)
                );
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to send email with template {Template}", template);
        }
    }
}
