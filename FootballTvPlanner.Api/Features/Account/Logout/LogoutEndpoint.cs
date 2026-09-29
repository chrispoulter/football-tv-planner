using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballTvPlanner.Api.Features.Account.Logout;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/logout", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Account)
            .WithSummary("Logout")
            .WithDescription("Sign the current user out.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        SignInManager<User> signInManager
    )
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }
}
