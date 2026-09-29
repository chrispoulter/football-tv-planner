using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Account.DeleteAccount;

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
        var user = await userManager.GetUserAsync(currentUser);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User not found."
            );
        }

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        await signInManager.SignOutAsync();

        return Results.Ok(new DeleteAccountResponse(user.Id));
    }
}
