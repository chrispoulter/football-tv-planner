using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.DisableTwoFactor;

public class DisableTwoFactorEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/two-factor/disable", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Disable Two-Factor")
            .WithDescription("Disable two-factor authentication for the current user.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.GetUserAsync(currentUser);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        await userManager.ResetAuthenticatorKeyAsync(user);
        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
