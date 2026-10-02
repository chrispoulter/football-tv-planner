using System.Text;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetFixtureCalendar;

public class GetFixtureCalendarEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/fixtures/{id:guid}/calendar.ics", HandleAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: "text/calendar")
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Fixture Calendar Event")
            .WithDescription(
                "Download a fixture as an iCalendar (.ics) event, with a reminder 30 minutes before kick-off."
            );
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        FootballTvPlannerDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var fixture = await dbContext
            .Fixtures.AsNoTracking()
            .Visible()
            .Where(f => f.Id == id)
            .Select(CalendarFixture.FromFixture())
            .FirstOrDefaultAsync(cancellationToken);

        if (fixture is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Fixture not found."
            );
        }

        var calendar = CalendarBuilder.Build([fixture], timeProvider.GetUtcNow());

        var fileName = $"{fixture.HomeTeam} v {fixture.AwayTeam}.ics";

        return Results.File(
            Encoding.UTF8.GetBytes(calendar),
            CalendarBuilder.ContentType,
            fileName
        );
    }
}
