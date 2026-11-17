using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Rebus.Handlers;

namespace FootballTvPlanner.Server.Features.Profile.ChangeEmail;

public class SendChangeEmailConfirmationHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings
) : IHandleMessages<SendChangeEmailConfirmation>
{
    public const string Template =
        "FootballTvPlanner.Server.Features.Account.Register.ConfirmEmail.html";

    public async Task Handle(SendChangeEmailConfirmation message)
    {
        var user = await userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null)
        {
            return;
        }

        var code = await userManager.GenerateChangeEmailTokenAsync(user, message.NewEmail);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/confirm-email",
            new Dictionary<string, string?>
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = code,
                ["changedEmail"] = message.NewEmail,
            }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: message.NewEmail,
            subject: "Verify your email address | Football TV Planner",
            template: Template,
            model: new { user.Name, Link = link }
        );
    }
}
