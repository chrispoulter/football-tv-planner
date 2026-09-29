using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Account.GetCurrentUser;

public class GetCurrentUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/account/me", HandleAsync)
            .RequireAuthorization()
            .Produces<CurrentUserResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Get Current User")
            .WithDescription("Get the profile and security settings of the current user.");
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
            new CurrentUserResponse(
                user.Id,
                user.Email!,
                user.Name,
                user.EmailConfirmed,
                await userManager.HasPasswordAsync(user),
                user.TwoFactorEnabled
            )
        );
    }
}
