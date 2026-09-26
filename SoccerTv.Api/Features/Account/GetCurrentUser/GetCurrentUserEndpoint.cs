using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.GetCurrentUser;

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
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
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
