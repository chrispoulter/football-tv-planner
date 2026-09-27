using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.TwoFactor.EnableTwoFactor;

public class EnableTwoFactorEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/two-factor/enable", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<EnableTwoFactorRequest>()
            .Produces<RecoveryCodesResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Enable Two-Factor")
            .WithDescription(
                "Enable two-factor authentication with a code from the authenticator app, returning a new set of recovery codes."
            );
    }

    private static async Task<IResult> HandleAsync(
        EnableTwoFactorRequest request,
        CurrentUser currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.FindByIdAsync(currentUser.Id.ToString());

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (user.TwoFactorEnabled)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Two-factor authentication is already enabled."
            );
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            request.Code
        );

        if (!isValid)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid two-factor authentication code."
            );
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, true);

        if (!result.Succeeded)
        {
            return Results.ValidationProblem(
                result
                    .Errors.GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())
            );
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(
            user,
            RecoveryCodesResponse.Count
        );

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok(new RecoveryCodesResponse(recoveryCodes ?? []));
    }
}
