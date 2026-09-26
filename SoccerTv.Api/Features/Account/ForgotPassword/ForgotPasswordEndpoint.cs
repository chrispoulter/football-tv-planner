using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
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
        AccountEmailSender emailSender
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            await emailSender.SendPasswordResetLinkAsync(user);
        }

        // Always succeed so the response doesn't reveal which emails have accounts
        return Results.Ok();
    }
}
