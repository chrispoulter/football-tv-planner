using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Fixtures.GetChannels;

public class GetChannelsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/channels", HandleAsync)
            .AllowAnonymous()
            .Produces<GetChannelsResponse>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Channels")
            .WithDescription(
                "List the TV channels and streaming services showing upcoming fixtures."
            );
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        HybridCache cache,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var response = await cache.GetOrCreateAsync(
            "channels",
            async cancel =>
            {
                var cutoff = timeProvider.GetUtcNow().AddDays(-1);

                var channels = await dbContext
                    .Channels.AsNoTracking()
                    .Where(c =>
                        dbContext.Fixtures.Any(f =>
                            f.KickoffUtc >= cutoff && f.Channels.Any(fc => fc.Id == c.Id)
                        )
                    )
                    .OrderBy(c => c.Name)
                    .Select(c => new GetChannelsItem(c.Id, c.Name))
                    .ToListAsync(cancel);

                return new GetChannelsResponse(channels);
            },
            cancellationToken: cancellationToken
        );

        return Results.Ok(response);
    }
}
