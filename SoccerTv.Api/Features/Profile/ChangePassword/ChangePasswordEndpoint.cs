using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.ChangePassword;

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
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        // Changing the password updates the security stamp, which would otherwise log the user out
        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
