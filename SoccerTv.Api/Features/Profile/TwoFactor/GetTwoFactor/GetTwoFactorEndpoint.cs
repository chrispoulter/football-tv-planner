using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.TwoFactor.GetTwoFactor;

public class GetTwoFactorEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/two-factor", HandleAsync)
            .RequireAuthorization()
            .Produces<TwoFactorResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Get Two-Factor")
            .WithDescription("Get the current user's two-factor authentication status.");
    }

    private static async Task<IResult> HandleAsync(
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

        return Results.Ok(
            new TwoFactorResponse(
                user.TwoFactorEnabled,
                await userManager.CountRecoveryCodesAsync(user),
                await signInManager.IsTwoFactorClientRememberedAsync(user)
            )
        );
    }
}
