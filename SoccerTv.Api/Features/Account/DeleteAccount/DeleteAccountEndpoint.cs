using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.DeleteProfile;

public class DeleteProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/profile", HandleAsync)
            .RequireAuthorization()
            .Produces<DeleteProfileResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Delete Profile")
            .WithDescription("Delete the profile of the current user.");
    }

    private static async Task<IResult> HandleAsync(
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

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return Results.ValidationProblem(
                result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
            );
        }

        await signInManager.SignOutAsync();

        return Results.Ok(new DeleteProfileResponse(user.Id));
    }
}
