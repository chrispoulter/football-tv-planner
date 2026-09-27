using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/forgot-password", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Forgot Password")
            .WithDescription(
                "Send a password reset link, if the email belongs to an account with a confirmed email address."
            );
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

        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var link = QueryHelpers.AddQueryString(
                $"{emailSettings.Value.SiteUrl}/account/reset-password",
                new Dictionary<string, string?> { ["email"] = user.Email, ["code"] = code }
            );

            await emailService.SendTemplateEmailAsync(
                toAddress: user.Email!,
                subject: "Reset Your Password",
                template: "SoccerTv.Api.Features.Emails.ResetPassword.html",
                model: new
                {
                    user.Name,
                    Link = link,
                },
                cancellationToken
            );
        }

        return Results.Ok();
    }
}
