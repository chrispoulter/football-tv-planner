using System.Security.Claims;
using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Account.ExternalLogin;

public class ExternalLoginCallbackEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/account/external-login/callback", HandleAsync).ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        string? returnUrl,
        HttpContext httpContext,
        SignInManager<User> signInManager,
        UserManager<User> userManager
    )
    {
        returnUrl = ExternalLoginRedirects.LocalOrRoot(returnUrl);

        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return Results.Redirect(ExternalLoginRedirects.LoginError("external"));
        }

        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true
        );

        if (result.Succeeded)
        {
            return Results.Redirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return Results.Redirect(ExternalLoginRedirects.LoginError("locked"));
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
        {
            return Results.Redirect(ExternalLoginRedirects.LoginError("external"));
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Name = info.GetName(),
            };

            if (!(await userManager.CreateAsync(user)).Succeeded)
            {
                return Results.Redirect(ExternalLoginRedirects.LoginError("external"));
            }
        }
        else
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return Results.Redirect(ExternalLoginRedirects.LoginError("locked"));
            }

            if (!user.EmailConfirmed)
            {
                if (await userManager.HasPasswordAsync(user))
                {
                    await userManager.RemovePasswordAsync(user);
                }

                user.EmailConfirmed = true;
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                user.Name = info.GetName();
            }

            await userManager.UpdateAsync(user);
        }

        if (!(await userManager.AddLoginAsync(user, info)).Succeeded)
        {
            return Results.Redirect(ExternalLoginRedirects.LoginError("external"));
        }

        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);

        return Results.Redirect(returnUrl);
    }
}
