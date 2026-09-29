using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.SetupTwoFactor;

public class SetupTwoFactorEndpoint : IEndpoint
{
    private const string Issuer = "Football TV Planner";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/two-factor/setup", HandleAsync)
            .RequireAuthorization()
            .Produces<SetupTwoFactorResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Setup Two-Factor")
            .WithDescription(
                "Get the key to add to an authenticator app, before enabling two-factor authentication with a code from the app."
            );
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
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

        if (user.TwoFactorEnabled)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Two-factor authentication is already enabled."
            );
        }

        var sharedKey = await userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(sharedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            await signInManager.RefreshSignInAsync(user);

            sharedKey = (await userManager.GetAuthenticatorKeyAsync(user))!;
        }

        var label = Uri.EscapeDataString($"{Issuer}:{user.Email}");
        var authenticatorUri =
            $"otpauth://totp/{label}?secret={sharedKey}&issuer={Uri.EscapeDataString(Issuer)}&digits=6";

        return Results.Ok(new SetupTwoFactorResponse(sharedKey, authenticatorUri));
    }
}
