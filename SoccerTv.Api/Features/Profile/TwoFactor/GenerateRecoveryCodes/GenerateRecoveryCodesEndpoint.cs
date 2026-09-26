using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.TwoFactor.GenerateRecoveryCodes;

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

    // Requiring a JSON body means a cross-site form post can't trigger this
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        if (!user.TwoFactorEnabled)
        {
            return AccountProblems.BadRequest("Two-factor authentication is not enabled.");
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(
            user,
            RecoveryCodesResponse.Count
        );

        return Results.Ok(new RecoveryCodesResponse(recoveryCodes ?? []));
    }
}
