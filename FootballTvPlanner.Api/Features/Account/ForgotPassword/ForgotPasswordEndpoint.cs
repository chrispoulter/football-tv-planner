using FootballTvPlanner.Api.Common.Email;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/forgot-password", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Forgot Password")
            .WithDescription("Send a password reset link, if the email belongs to an account.");
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        UserManager<User> userManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var link = QueryHelpers.AddQueryString(
                $"{emailSettings.Value.SiteUrl}/reset-password",
                new Dictionary<string, string?> { ["email"] = user.Email, ["code"] = code }
            );

            await emailService.SendTemplateEmailAsync(
                toAddress: user.Email!,
                subject: "Reset Your Password",
                template: "FootballTvPlanner.Api.Features.Account.Emails.ResetPassword.html",
                model: new { user.Name, Link = link },
                cancellationToken
            );
        }

        return Results.Ok();
    }
}
