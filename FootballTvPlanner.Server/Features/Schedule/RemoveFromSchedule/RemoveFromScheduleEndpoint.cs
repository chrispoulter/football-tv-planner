using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Features.Schedule.GetScheduleCalendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Schedule.RemoveFromSchedule;

public class RemoveFromScheduleEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/schedule/{fixtureId:guid}", HandleAsync)
            .RequireAuthorization()
            .Produces<ScheduleItemResponse>()
            .WithTags(Tags.Schedule)
            .WithSummary("Remove From Schedule")
            .WithDescription("Remove a fixture from the current user's schedule.");
    }

    private static async Task<IResult> HandleAsync(
        Guid fixtureId,
        CurrentUser currentUser,
        FootballTvPlannerDbContext dbContext,
        HybridCache cache,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .UserFixtures.IgnoreQueryFilters()
            .Where(uf => uf.UserId == currentUser.Id && uf.FixtureId == fixtureId)
            .ExecuteDeleteAsync(cancellationToken);

        await cache.RemoveByTagAsync($"user:{currentUser.Id}", cancellationToken);

        return Results.Ok(new ScheduleItemResponse(fixtureId));
    }
}
