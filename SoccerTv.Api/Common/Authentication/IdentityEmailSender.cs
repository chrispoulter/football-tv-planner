using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Common.Authentication;

/// <summary>
/// Sends the Identity API emails with links to the SPA rather than the API.
/// MapIdentityApi resolves this from the root provider, so it's a singleton that creates a scope
/// for the scoped email service.
/// </summary>
public class IdentityEmailSender(
    IServiceScopeFactory scopeFactory,
    IOptions<EmailSettings> emailSettings
) : IEmailSender<User>
{
    private const string TemplatePrefix = "SoccerTv.Api.Common.Authentication.Emails";

    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        // Identity HTML-encodes the link and points it at the API's /confirmEmail endpoint
        var apiLink = new Uri(WebUtility.HtmlDecode(confirmationLink));
        var query = QueryHelpers.ParseQuery(apiLink.Query);

        var link = QueryHelpers.AddQueryString(
            $"{_emailSettings.SiteUrl}/account/confirm-email",
            query.ToDictionary(q => q.Key, q => (string?)q.Value.ToString())
        );

        await SendAsync(email, "Confirm your email // SoccerTv", "ConfirmEmail.html", link);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        var link = QueryHelpers.AddQueryString(
            $"{_emailSettings.SiteUrl}/account/reset-password",
            new Dictionary<string, string?> { ["email"] = email, ["code"] = resetCode }
        );

        await SendAsync(email, "Reset Password // SoccerTv", "ResetPassword.html", link);
    }

    private async Task SendAsync(string email, string subject, string template, string link)
    {
        using var scope = scopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        await emailService.SendTemplateEmailAsync(
            toAddress: email,
            subject: subject,
            template: $"{TemplatePrefix}.{template}",
            model: new { Link = link }
        );
    }

    // Not used by MapIdentityApi, which only sends reset codes
    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) =>
        Task.CompletedTask;
}
