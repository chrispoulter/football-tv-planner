using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.UpdateProfile;

public class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/profile", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<UpdateProfileRequest>()
            .WithTags(Tags.Profile)
            .WithSummary("Update Profile")
            .WithDescription("Update the personal details of the current user.");
    }

    private static async Task<IResult> HandleAsync(
        UpdateProfileRequest request,
        CurrentUser currentUser,
        UserManager<User> userManager
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

        user.Name = request.Name;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        return Results.Ok();
    }
}
