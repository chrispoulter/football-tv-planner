using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.GetTwoFactor;

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
        UserManager<User> userManager
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

        return Results.Ok(
            new TwoFactorResponse(
                user.TwoFactorEnabled,
                await userManager.CountRecoveryCodesAsync(user)
            )
        );
    }
}
