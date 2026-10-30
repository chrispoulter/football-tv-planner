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
        IServiceScopeFactory scopeFactory,
        HybridCache cache,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var channels = await cache.GetOrCreateAsync(
            "channels",
            async cancel =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var cacheDbContext =
                    scope.ServiceProvider.GetRequiredService<FootballTvPlannerDbContext>();

                var cutoff = timeProvider.GetUtcNow().AddDays(-1);

                var channels = await cacheDbContext
                    .Channels.AsNoTracking()
                    .Where(c =>
                        cacheDbContext.Fixtures.Any(f =>
                            f.KickoffUtc >= cutoff && f.Channels.Any(fc => fc.Id == c.Id)
                        )
                    )
                    .OrderBy(c => c.Name)
                    .Select(c => new GetChannelsItem(c.Id, c.Name))
                    .ToListAsync(cancel);

                return channels;
            },
            cancellationToken: cancellationToken
        );

        return Results.Ok(new GetChannelsResponse(channels));
    }
}
