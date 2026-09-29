using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.ForgetTwoFactorMachine;

public class ForgetTwoFactorMachineEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/two-factor/forget-machine", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Forget Two-Factor Machine")
            .WithDescription("Ask for a two-factor code the next time this browser logs in.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        SignInManager<User> signInManager
    )
    {
        await signInManager.ForgetTwoFactorClientAsync();

        return Results.Ok();
    }
}
