using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Common.Authentication;

/// <summary>
/// Sends the account emails, with links to the pages on the web app that complete each action.
/// </summary>
public class AccountEmailSender(
    UserManager<User> userManager,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings
)
{
    private const string TemplatePrefix = "SoccerTv.Api.Common.Authentication.Emails";

    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendConfirmationLinkAsync(User user)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{_emailSettings.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?>
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = AccountCodes.Encode(code),
            }
        );

        await SendAsync(user.Email!, "Confirm your email // SoccerTv", "ConfirmEmail.html", link);
    }

    public async Task SendChangeEmailLinkAsync(User user, string newEmail)
    {
        var code = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        var link = QueryHelpers.AddQueryString(
            $"{_emailSettings.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?>
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = AccountCodes.Encode(code),
                ["changedEmail"] = newEmail,
            }
        );

        await SendAsync(newEmail, "Confirm your email // SoccerTv", "ConfirmEmail.html", link);
    }

    public async Task SendPasswordResetLinkAsync(User user)
    {
        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{_emailSettings.SiteUrl}/account/reset-password",
            new Dictionary<string, string?>
            {
                ["email"] = user.Email,
                ["code"] = AccountCodes.Encode(code),
            }
        );

        await SendAsync(user.Email!, "Reset Password // SoccerTv", "ResetPassword.html", link);
    }

    private async Task SendAsync(string email, string subject, string template, string link)
    {
        await emailService.SendTemplateEmailAsync(
            toAddress: email,
            subject: subject,
            template: $"{TemplatePrefix}.{template}",
            model: new { Link = link, _emailSettings.SiteUrl }
        );
    }
}
