using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ForgetTwoFactorMachine;

public class ForgetTwoFactorMachineEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/two-factor/forget-machine", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Account)
            .WithSummary("Forget Two-Factor Machine")
            .WithDescription("Ask for a two-factor code the next time this browser logs in.");
    }

    // Requiring a JSON body means a cross-site form post can't trigger this
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        SignInManager<User> signInManager
    )
    {
        await signInManager.ForgetTwoFactorClientAsync();

        return Results.Ok();
    }
}
