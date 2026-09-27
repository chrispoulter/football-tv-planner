using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Schedule;

namespace SoccerTv.Api.Features.Schedule.AddToSchedule;

public class AddToScheduleEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/schedule/{fixtureId:guid}", HandleAsync)
            .RequireAuthorization()
            .Produces<ScheduleItemResponse>()
            .WithTags(Tags.Schedule)
            .WithSummary("Add To Schedule")
            .WithDescription("Bookmark a fixture in the current user's schedule.");
    }

    private static async Task<IResult> HandleAsync(
        Guid fixtureId,
        CurrentUser currentUser,
        SoccerTvDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken
    )
    {
        var fixtureExists = await dbContext.Fixtures.AnyAsync(
            f => f.Id == fixtureId,
            cancellationToken
        );

        if (!fixtureExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Fixture not found."
            );
        }

        var alreadyAdded = await dbContext.UserFixtures.AnyAsync(
            uf => uf.UserId == currentUser.Id && uf.FixtureId == fixtureId,
            cancellationToken
        );

        if (!alreadyAdded)
        {
            dbContext.UserFixtures.Add(
                new UserFixture
                {
                    UserId = currentUser.Id,
                    FixtureId = fixtureId,
                    CreatedAt = timeProvider.GetUtcNow(),
                }
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Results.Ok(new ScheduleItemResponse(fixtureId));
    }
}
