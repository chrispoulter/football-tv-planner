using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Logout;

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

    // Requiring a JSON body means a cross-site form post can't trigger a logout
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        SignInManager<User> signInManager
    )
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }
}
