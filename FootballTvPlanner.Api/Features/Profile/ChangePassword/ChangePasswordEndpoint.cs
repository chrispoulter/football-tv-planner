using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Profile.ChangePassword;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/change-password", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<ChangePasswordRequest>()
            .WithTags(Tags.Profile)
            .WithSummary("Change Password")
            .WithDescription("Change the current user's password.");
    }

    private static async Task<IResult> HandleAsync(
        ChangePasswordRequest request,
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

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
