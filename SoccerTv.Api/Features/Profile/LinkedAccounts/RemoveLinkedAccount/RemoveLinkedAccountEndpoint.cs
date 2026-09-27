using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.LinkedAccounts.RemoveLinkedAccount;

public class RemoveLinkedAccountEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/profile/linked-accounts/{provider}", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Remove Linked Account")
            .WithDescription(
                "Unlink an external login provider, such as Google, from the current user."
            );
    }

    private static async Task<IResult> HandleAsync(
        string provider,
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

        var logins = await userManager.GetLoginsAsync(user);
        var login = logins.FirstOrDefault(l => l.LoginProvider == provider);

        if (login is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Linked account not found."
            );
        }

        if (logins.Count == 1 && !await userManager.HasPasswordAsync(user))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Cannot remove the only linked account without a password set."
            );
        }

        var result = await userManager.RemoveLoginAsync(
            user,
            login.LoginProvider,
            login.ProviderKey
        );

        if (!result.Succeeded)
        {
            return Results.ValidationProblem(
                result
                    .Errors.GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())
            );
        }

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
