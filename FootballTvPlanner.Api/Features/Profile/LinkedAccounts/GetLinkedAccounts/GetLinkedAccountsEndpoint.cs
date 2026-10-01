using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Profile.LinkedAccounts.GetLinkedAccounts;

public class GetLinkedAccountsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/linked-accounts", HandleAsync)
            .RequireAuthorization()
            .Produces<LinkedAccountsResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Get Linked Accounts")
            .WithDescription(
                "List the external login providers, such as Google, that the current user has linked."
            );
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

        var logins = await userManager.GetLoginsAsync(user);

        return Results.Ok(new LinkedAccountsResponse(logins.Select(l => l.LoginProvider)));
    }
}
