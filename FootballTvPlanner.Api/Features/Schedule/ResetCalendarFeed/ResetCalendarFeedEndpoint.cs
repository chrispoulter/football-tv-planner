using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Schedule.ResetCalendarFeed;

public class ResetCalendarFeedEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/schedule/calendar-feed/reset", HandleAsync)
            .RequireAuthorization()
            .Produces<CalendarFeedResponse>()
            .WithTags(Tags.Schedule)
            .WithSummary("Reset Calendar Feed")
            .WithDescription(
                "Generate a new calendar subscription URL. The previous URL stops working."
            );
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        HttpRequest httpRequest,
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == currentUser.Id,
            cancellationToken
        );

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        user.CalendarFeedToken = CalendarFeedResponse.GenerateToken();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(CalendarFeedResponse.Create(httpRequest, user.CalendarFeedToken));
    }
}
