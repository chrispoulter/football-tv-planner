using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/reset-password", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<ResetPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Reset Password")
            .WithDescription("Set a new password using the link from the password reset email.");
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        UserManager<User> userManager
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        var code = AccountCodes.Decode(request.Code);

        // Links are only sent to confirmed emails, so treat anything else as an invalid link
        if (user is null || code is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return InvalidLink();
        }

        var result = await userManager.ResetPasswordAsync(user, code, request.NewPassword);

        if (!result.Succeeded)
        {
            return result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.InvalidToken))
                ? InvalidLink()
                : result.ToValidationProblem();
        }

        return Results.Ok();
    }

    private static IResult InvalidLink() =>
        AccountProblems.BadRequest("This link is invalid or has expired.");
}
