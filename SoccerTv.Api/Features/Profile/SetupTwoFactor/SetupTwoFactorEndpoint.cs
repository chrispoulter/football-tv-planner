using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.SetupTwoFactor;

public class SetupTwoFactorEndpoint : IEndpoint
{
    private const string Issuer = "SoccerTv";

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

    // Requiring a JSON body means a cross-site form post can't trigger this
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
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

        var sharedKey = await userManager.GetAuthenticatorKeyAsync(user);

        // Keep an existing key, so setup can be restarted without rescanning the QR code
        if (string.IsNullOrEmpty(sharedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            await signInManager.RefreshSignInAsync(user);

            sharedKey = (await userManager.GetAuthenticatorKeyAsync(user))!;
        }

        var label = Uri.EscapeDataString($"{Issuer}:{user.Email}");
        var authenticatorUri =
            $"otpauth://totp/{label}?secret={sharedKey}&issuer={Issuer}&digits=6";

        return Results.Ok(new SetupTwoFactorResponse(sharedKey, authenticatorUri));
    }
}
