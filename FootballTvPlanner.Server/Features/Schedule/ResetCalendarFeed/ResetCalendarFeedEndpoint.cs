using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Schedule.ResetCalendarFeed;

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
        HybridCache cache,
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

        var previousToken = user.CalendarFeedToken;

        user.CalendarFeedToken = CalendarFeedResponse.GenerateToken();
        await dbContext.SaveChangesAsync(cancellationToken);

        if (previousToken is not null)
        {
            await cache.RemoveByTagAsync($"token:{previousToken}", cancellationToken);
        }

        return Results.Ok(CalendarFeedResponse.Create(httpRequest, user.CalendarFeedToken));
    }
}
