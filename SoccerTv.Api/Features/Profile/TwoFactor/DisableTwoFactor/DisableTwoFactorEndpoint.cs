using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.TwoFactor.DisableTwoFactor;

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

    // Requiring a JSON body means a cross-site form post can't trigger this
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        // Re-enabling starts again with a new key, so the old authenticator entry stops working
        await userManager.ResetAuthenticatorKeyAsync(user);
        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
