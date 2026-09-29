using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Profile.LinkedAccounts.LinkAccount;

public class LinkAccountCallbackEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/linked-accounts/link/callback", HandleAsync)
            .RequireAuthorization()
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        string? returnUrl,
        CurrentUser currentUser,
        HttpContext httpContext,
        SignInManager<User> signInManager,
        UserManager<User> userManager
    )
    {
        returnUrl = ExternalLoginRedirects.LocalOrRoot(returnUrl);

        var user = await userManager.GetUserAsync(currentUser);
        var info = await signInManager.GetExternalLoginInfoAsync(currentUser.Id.ToString());

        if (user is null || info is null)
        {
            return Results.Redirect(ExternalLoginRedirects.WithError(returnUrl, "link"));
        }

        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var result = await userManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            var error = result.Errors.Any(e =>
                e.Code == nameof(IdentityErrorDescriber.LoginAlreadyAssociated)
            )
                ? "linked"
                : "link";

            return Results.Redirect(ExternalLoginRedirects.WithError(returnUrl, error));
        }

        if (string.IsNullOrEmpty(user.Name))
        {
            user.Name = info.GetName();
            await userManager.UpdateAsync(user);
        }

        await signInManager.RefreshSignInAsync(user);

        return Results.Redirect(returnUrl);
    }
}
