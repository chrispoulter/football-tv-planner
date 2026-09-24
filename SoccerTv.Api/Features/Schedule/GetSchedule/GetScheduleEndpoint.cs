using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;
using SoccerTv.Api.Features.Fixtures;

namespace SoccerTv.Api.Features.Schedule.GetSchedule;

public class GetScheduleEndpoint : IEndpoint
{
    /// <summary>
    /// Keep games that are still being played in the upcoming list.
    /// </summary>
    private static readonly TimeSpan InProgressWindow = TimeSpan.FromHours(3);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/schedule", HandleAsync)
            .RequireAuthorization()
            .Produces<GetScheduleResponse>()
            .WithTags(Tags.Schedule)
            .WithSummary("Get Schedule")
            .WithDescription("List the fixtures the current user has added to their schedule.");
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetScheduleRequest request,
        CurrentUser currentUser,
        SoccerTvDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext
            .UserFixtures.AsNoTracking()
            .Where(uf => uf.UserId == currentUser.Id)
            .Select(uf => uf.Fixture);

        if (request.IncludePast != true)
        {
            var cutoff = timeProvider.GetUtcNow() - InProgressWindow;
            query = query.Where(f => f.KickoffUtc >= cutoff);
        }

        var fixtures = await query
            .OrderBy(f => f.KickoffUtc)
            .ThenBy(f => f.Competition.SortOrder)
            .Select(FixtureProjections.ToSummary(dbContext, currentUser.Id))
            .ToListAsync(cancellationToken);

        return Results.Ok(new GetScheduleResponse(fixtures));
    }
}
