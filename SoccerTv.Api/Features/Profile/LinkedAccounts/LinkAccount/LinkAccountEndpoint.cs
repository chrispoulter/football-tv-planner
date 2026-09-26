using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;
using SoccerTv.Api.Features.Account.ExternalLogin;

namespace SoccerTv.Api.Features.Profile.LinkedAccounts.LinkAccount;

public class LinkAccountEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/linked-accounts/link", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Link Account")
            .WithDescription(
                "Redirect to an external login provider such as Google, to link it to the current user."
            );
    }

    private static async Task<IResult> HandleAsync(
        string provider,
        string? returnUrl,
        CurrentUser currentUser,
        HttpContext httpContext,
        SignInManager<User> signInManager
    )
    {
        var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();

        if (!schemes.Any(s => s.Name == provider))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Unknown login provider."
            );
        }

        // Clear any external login left over from an earlier attempt
        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var callbackUrl = QueryHelpers.AddQueryString(
            $"{httpContext.Request.PathBase}/profile/linked-accounts/link/callback",
            "returnUrl",
            ExternalLoginRedirects.LocalOrRoot(returnUrl)
        );

        // Including the user ID means the callback only accepts this user's login
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            callbackUrl,
            currentUser.Id.ToString()
        );

        return Results.Challenge(properties, [provider]);
    }
}
