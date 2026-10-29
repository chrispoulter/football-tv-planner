using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Features.Account.GetCurrentUser;

public class GetCurrentUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/account/me", HandleAsync)
            .AllowAnonymous()
            .Produces<CurrentUserResponse>()
            .Produces(StatusCodes.Status204NoContent)
            .WithTags(Tags.Account)
            .WithSummary("Get Current User")
            .WithDescription(
                "Get the profile and security settings of the current user, or no content if nobody is signed in."
            );
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser? currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        if (currentUser is null)
        {
            return Results.NoContent();
        }

        var user = await userManager.GetUserAsync(currentUser);

        if (user is null)
        {
            await signInManager.SignOutAsync();
            return Results.NoContent();
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
