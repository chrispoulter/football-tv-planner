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

        await SendAsync(
            user,
            user.Email!,
            "Verify your email address | SoccerTv",
            "ConfirmEmail.html",
            link
        );
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

        await SendAsync(
            user,
            newEmail,
            "Verify your email address | SoccerTv",
            "ConfirmEmail.html",
            link
        );
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

        await SendAsync(
            user,
            user.Email!,
            "Reset your password | SoccerTv",
            "ResetPassword.html",
            link
        );
    }

    private async Task SendAsync(
        User user,
        string email,
        string subject,
        string template,
        string link
    )
    {
        await emailService.SendTemplateEmailAsync(
            toAddress: email,
            subject: subject,
            template: $"{TemplatePrefix}.{template}",
            model: new
            {
                Link = link,
                Name = string.IsNullOrWhiteSpace(user.Name) ? email : user.Name,
                _emailSettings.SiteUrl,
            }
        );
    }
}
