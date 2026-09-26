using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.DeleteAccount;

public class DeleteAccountEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/account", HandleAsync)
            .RequireAuthorization()
            .Produces<DeleteAccountResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Delete Account")
            .WithDescription("Delete the account of the current user.");
    }

    private static async Task<IResult> HandleAsync(
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

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        await signInManager.SignOutAsync();

        return Results.Ok(new DeleteAccountResponse(user.Id));
    }
}
