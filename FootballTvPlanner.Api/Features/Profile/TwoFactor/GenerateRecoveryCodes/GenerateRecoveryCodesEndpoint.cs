using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.GenerateRecoveryCodes;

public class GenerateRecoveryCodesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/two-factor/recovery-codes", HandleAsync)
            .RequireAuthorization()
            .Produces<RecoveryCodesResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Generate Recovery Codes")
            .WithDescription(
                "Generate a new set of two-factor recovery codes. The previous codes stop working."
            );
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
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

        if (!user.TwoFactorEnabled)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Two-factor authentication is not enabled."
            );
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(
            user,
            RecoveryCodesResponse.Count
        );

        return Results.Ok(new RecoveryCodesResponse(recoveryCodes ?? []));
    }
}
