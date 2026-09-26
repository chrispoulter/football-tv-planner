using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.LoginTwoFactor;

public class LoginTwoFactorEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/login/two-factor", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<LoginTwoFactorRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Login Two-Factor")
            .WithDescription(
                "Complete a login that requires two-factor authentication with a code from the authenticator app or a recovery code."
            );
    }

    private static async Task<IResult> HandleAsync(
        LoginTwoFactorRequest request,
        SignInManager<User> signInManager
    )
    {
        // Set by the login endpoint, and only valid for a few minutes
        if (await signInManager.GetTwoFactorAuthenticationUserAsync() is null)
        {
            return AccountProblems.Unauthorized("Your login has expired, please log in again.");
        }

        var result = string.IsNullOrEmpty(request.RecoveryCode)
            ? await signInManager.TwoFactorAuthenticatorSignInAsync(
                AccountCodes.NormalizeAuthenticatorCode(request.Code!),
                request.RememberMe,
                request.RememberMachine
            )
            : await signInManager.TwoFactorRecoveryCodeSignInAsync(
                AccountCodes.NormalizeRecoveryCode(request.RecoveryCode)
            );

        if (result.IsLockedOut)
        {
            return AccountProblems.Unauthorized(
                "This account has been locked out, please try again later."
            );
        }

        if (!result.Succeeded)
        {
            return AccountProblems.Unauthorized("The code provided was invalid.");
        }

        return Results.Ok();
    }
}
