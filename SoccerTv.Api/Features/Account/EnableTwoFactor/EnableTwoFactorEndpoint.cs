using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.EnableTwoFactor;

public class EnableTwoFactorEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/two-factor/enable", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<EnableTwoFactorRequest>()
            .Produces<RecoveryCodesResponse>()
            .WithTags(Tags.Account)
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
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        if (user.TwoFactorEnabled)
        {
            return AccountProblems.BadRequest("Two-factor authentication is already enabled.");
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            AccountCodes.NormalizeAuthenticatorCode(request.Code)
        );

        if (!isValid)
        {
            return AccountProblems.Validation(
                nameof(request.Code),
                "The code provided was invalid."
            );
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, true);

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        // Always issue fresh recovery codes, as any old ones survive disabling 2FA
        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(
            user,
            RecoveryCodesResponse.Count
        );

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok(new RecoveryCodesResponse(recoveryCodes ?? []));
    }
}
