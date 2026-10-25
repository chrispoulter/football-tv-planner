using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Common.Validation;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Features.Account.LoginTwoFactor;

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
        if (await signInManager.GetTwoFactorAuthenticationUserAsync() is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Your login has expired, please log in again."
            );
        }

        var result = string.IsNullOrEmpty(request.RecoveryCode)
            ? await signInManager.TwoFactorAuthenticatorSignInAsync(
                request.Code!,
                request.RememberMe,
                rememberClient: false
            )
            : await signInManager.TwoFactorRecoveryCodeSignInAsync(
                request.RecoveryCode.Replace(" ", string.Empty)
            );

        if (result.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This account has been locked out, please try again later."
            );
        }

        if (!result.Succeeded)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The code provided was invalid."
            );
        }

        return Results.Ok();
    }
}
