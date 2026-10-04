using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetChannels;

public class GetChannelsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/channels", HandleAsync)
            .AllowAnonymous()
            .Produces<GetChannelsResponse>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Channels")
            .WithDescription("List the TV channels and streaming services showing fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var channels = await dbContext
            .Channels.AsNoTracking()
            .Where(c => dbContext.Fixtures.Any(f => f.Channels.Any(fc => fc.Id == c.Id)))
            .OrderBy(c => c.Name)
            .Select(c => new GetChannelsItem(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return Results.Ok(new GetChannelsResponse(channels));
    }
}
