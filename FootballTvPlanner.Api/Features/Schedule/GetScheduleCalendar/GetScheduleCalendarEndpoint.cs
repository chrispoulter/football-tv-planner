using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Features.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Schedule.GetScheduleCalendar;

public class GetScheduleCalendarEndpoint : IEndpoint
{
    private static readonly TimeSpan HistoryWindow = TimeSpan.FromDays(7);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/calendar/{token}.ics", HandleAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: "text/calendar")
            .WithTags(Tags.Schedule)
            .WithSummary("Get Schedule Calendar Feed")
            .WithDescription(
                "iCalendar subscription feed of a user's schedule. The token in the URL is the only credential, so calendar apps can poll it."
            );
    }

    private static async Task<IResult> HandleAsync(
        string token,
        FootballTvPlannerDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken
    )
    {
        var userId = await dbContext
            .Users.AsNoTracking()
            .Where(u => u.CalendarFeedToken == token)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Calendar not found."
            );
        }

        var now = timeProvider.GetUtcNow();
        var cutoff = now - HistoryWindow;

        var fixtures = await dbContext
            .UserFixtures.AsNoTracking()
            .Where(uf => uf.UserId == userId && uf.Fixture.KickoffUtc >= cutoff)
            .Select(uf => uf.Fixture)
            .OrderBy(f => f.KickoffUtc)
            .Select(FixtureProjections.ToCalendarFixture())
            .ToListAsync(cancellationToken);

        var calendar = CalendarBuilder.Build(fixtures, now, calendarName: "Football TV Planner");

        return Results.Text(calendar, CalendarBuilder.ContentType);
    }
}
