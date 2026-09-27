using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Schedule.GetCalendarFeed;

public class GetCalendarFeedEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/schedule/calendar-feed", HandleAsync)
            .RequireAuthorization()
            .Produces<CalendarFeedResponse>()
            .WithTags(Tags.Schedule)
            .WithSummary("Get Calendar Feed")
            .WithDescription(
                "Get the private calendar subscription URL for the current user's schedule, creating it if needed."
            );
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        HttpRequest httpRequest,
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken
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

        if (user.CalendarFeedToken is null)
        {
            user.CalendarFeedToken = CalendarFeedResponse.GenerateToken();
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Results.Ok(CalendarFeedResponse.Create(httpRequest, user.CalendarFeedToken));
    }
}
