using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace FootballTvPlanner.Server.Features.Account.ConfirmEmail;

public static class ConfirmationEmail
{
    public static async Task SendAsync(
        User user,
        UserManager<User> userManager,
        IEmailService emailService,
        EmailSettings emailSettings,
        CancellationToken cancellationToken
    )
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.SiteUrl}/confirm-email",
            new Dictionary<string, string?> { ["userId"] = user.Id.ToString(), ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email!,
            subject: "Verify your email address | Football TV Planner",
            template: "FootballTvPlanner.Server.Features.Account.Emails.ConfirmEmail.html",
            model: new { user.Name, Link = link },
            cancellationToken
        );
    }
}
