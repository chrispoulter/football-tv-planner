using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.UpdateProfile;

public class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/profile", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<UpdateProfileRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Update Profile")
            .WithDescription("Update the personal details of the current user.");
    }

    private static async Task<IResult> HandleAsync(
        UpdateProfileRequest request,
        CurrentUser currentUser,
        UserManager<User> userManager
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        user.Name = request.Name.Trim();

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return result.ToValidationProblem();
        }

        return Results.Ok();
    }
}
