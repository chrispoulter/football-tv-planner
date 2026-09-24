using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Profile.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile", HandleAsync)
            .RequireAuthorization()
            .Produces<GetProfileResponse>()
            .WithTags(Tags.Profile)
            .WithSummary("Get Profile")
            .WithDescription("Retrieve the profile of the current user.");
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken);

        if (user is null || user.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = new GetProfileResponse(
            user.Id,
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            user.DateOfBirth,
            user.ReminderMinutesBefore
        );

        return Results.Ok(result);
    }
}
