using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Rebus.Handlers;

namespace FootballTvPlanner.Server.Features.Account.ForgotPassword;

public class ForgotPasswordRequestedHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings
) : IHandleMessages<ForgotPasswordRequested>
{
    public async Task Handle(ForgotPasswordRequested message)
    {
        var user = await userManager.FindByEmailAsync(message.Email);

        if (user is null)
        {
            return;
        }

        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/reset-password",
            new Dictionary<string, string?> { ["email"] = user.Email, ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email!,
            subject: "Reset your password | Football TV Planner",
            template: "FootballTvPlanner.Server.Features.Account.ForgotPassword.ResetPassword.html",
            model: new { user.Name, Link = link }
        );
    }
}
