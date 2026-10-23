using FootballTvPlanner.Data;
using FootballTvPlanner.Data.Schedule;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Schedule.AddToSchedule;

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
        FootballTvPlannerDbContext dbContext,
        HybridCache cache,
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

            await cache.RemoveByTagAsync($"user:{currentUser.Id}", cancellationToken);
        }

        return Results.Ok(new ScheduleItemResponse(fixtureId));
    }
}
