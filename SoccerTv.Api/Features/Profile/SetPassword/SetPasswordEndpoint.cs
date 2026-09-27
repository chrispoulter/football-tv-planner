using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.SetPassword;

public class SetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/set-password", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<SetPasswordRequest>()
            .WithTags(Tags.Profile)
            .WithSummary("Set Password")
            .WithDescription(
                "Add a password to an account that only logs in with an external provider such as Google."
            );
    }

    private static async Task<IResult> HandleAsync(
        SetPasswordRequest request,
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

        if (await userManager.HasPasswordAsync(user))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Your account already has a password."
            );
        }

        var result = await userManager.AddPasswordAsync(user, request.NewPassword);

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        await signInManager.RefreshSignInAsync(user);

        return Results.Ok();
    }
}
