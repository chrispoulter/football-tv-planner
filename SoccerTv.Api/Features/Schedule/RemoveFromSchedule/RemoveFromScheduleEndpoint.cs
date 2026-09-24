using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Schedule.RemoveFromSchedule;

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
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .UserFixtures.Where(uf => uf.UserId == currentUser.Id && uf.FixtureId == fixtureId)
            .ExecuteDeleteAsync(cancellationToken);

        return Results.Ok(new ScheduleItemResponse(fixtureId));
    }
}
