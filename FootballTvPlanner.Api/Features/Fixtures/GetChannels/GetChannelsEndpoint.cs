using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Channels;
using FootballTvPlanner.Api.Data.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetChannels;

public class GetChannelsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/channels", HandleAsync)
            .AllowAnonymous()
            .Produces<List<ChannelResponse>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Channels")
            .WithDescription("List the TV channels and streaming services showing fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var fixtures = dbContext.Fixtures.Visible();

        var channels = await dbContext
            .Channels.AsNoTracking()
            .Where(c =>
                !c.IsExcluded
                && fixtures.Any(f => f.FixtureChannels.Any(fc => fc.ChannelId == c.Id))
            )
            .InDisplayOrder()
            .Select(c => new ChannelResponse(c.Id, c.DisplayName ?? c.Name))
            .ToListAsync(cancellationToken);

        return Results.Ok(channels);
    }
}
