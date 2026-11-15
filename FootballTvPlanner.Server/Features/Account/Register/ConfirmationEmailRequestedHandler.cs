using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Rebus.Handlers;

namespace FootballTvPlanner.Server.Features.Account.Register;

public class ConfirmationEmailRequestedHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings
) : IHandleMessages<ConfirmationEmailRequested>
{
    public const string Template =
        "FootballTvPlanner.Server.Features.Account.Register.ConfirmEmail.html";

    public async Task Handle(ConfirmationEmailRequested message)
    {
        var user = await userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null || user.EmailConfirmed)
        {
            return;
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/confirm-email",
            new Dictionary<string, string?> { ["userId"] = user.Id.ToString(), ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email!,
            subject: "Verify your email address | Football TV Planner",
            template: Template,
            model: new { user.Name, Link = link }
        );
    }
}
