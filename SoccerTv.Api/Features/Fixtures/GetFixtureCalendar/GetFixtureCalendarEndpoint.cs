using System.Text;
using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Calendar;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Fixtures.GetFixtureCalendar;

public class GetFixtureCalendarEndpoint : IEndpoint
{
    private const int DefaultReminderMinutesBefore = 30;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/fixtures/{id:guid}/calendar.ics", HandleAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: "text/calendar")
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Fixture Calendar Event")
            .WithDescription(
                "Download a fixture as an iCalendar (.ics) event, with a reminder based on the current user's settings."
            );
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        CurrentUser? currentUser,
        SoccerTvDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var fixture = await dbContext
            .Fixtures.AsNoTracking()
            .Where(f => f.Id == id)
            .Select(FixtureProjections.ToCalendarFixture())
            .FirstOrDefaultAsync(cancellationToken);

        if (fixture is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Fixture not found."
            );
        }

        var reminderMinutesBefore = DefaultReminderMinutesBefore;

        if (currentUser is not null)
        {
            reminderMinutesBefore =
                await dbContext
                    .Users.Where(u => u.Id == currentUser.Id)
                    .Select(u => (int?)u.ReminderMinutesBefore)
                    .FirstOrDefaultAsync(cancellationToken)
                ?? DefaultReminderMinutesBefore;
        }

        var calendar = CalendarBuilder.Build(
            [fixture],
            reminderMinutesBefore,
            timeProvider.GetUtcNow()
        );

        var fileName = $"{fixture.HomeTeam} v {fixture.AwayTeam}.ics";

        return Results.File(
            Encoding.UTF8.GetBytes(calendar),
            CalendarBuilder.ContentType,
            fileName
        );
    }
}
