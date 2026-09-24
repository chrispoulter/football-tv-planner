using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Calendar;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;
using SoccerTv.Api.Features.Fixtures;

namespace SoccerTv.Api.Features.Schedule.GetScheduleCalendar;

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
        SoccerTvDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext
            .Users.AsNoTracking()
            .Where(u => u.CalendarFeedToken == token && !u.IsLockedOut)
            .Select(u => new { u.Id, u.ReminderMinutesBefore })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
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
            .Where(uf => uf.UserId == user.Id && uf.Fixture.KickoffUtc >= cutoff)
            .Select(uf => uf.Fixture)
            .OrderBy(f => f.KickoffUtc)
            .Select(FixtureProjections.ToCalendarFixture())
            .ToListAsync(cancellationToken);

        var calendar = CalendarBuilder.Build(
            fixtures,
            user.ReminderMinutesBefore,
            now,
            calendarName: "My Soccer on TV"
        );

        return Results.Text(calendar, CalendarBuilder.ContentType);
    }
}
