using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.RemoveLinkedAccount;

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
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        var logins = await userManager.GetLoginsAsync(user);
        var login = logins.FirstOrDefault(l => l.LoginProvider == provider);

        if (login is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Linked account not found."
            );
        }

        if (logins.Count == 1 && !await userManager.HasPasswordAsync(user))
        {
            return AccountProblems.BadRequest(
                "Set a password before unlinking your only way to log in."
            );
        }

        var result = await userManager.RemoveLoginAsync(
            user,
            login.LoginProvider,
            login.ProviderKey
        );

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
