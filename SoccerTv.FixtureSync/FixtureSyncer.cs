using Microsoft.EntityFrameworkCore;
using SoccerTv.FixtureSync.Data;
using SoccerTv.FixtureSync.Providers;

namespace SoccerTv.FixtureSync;

public class FixtureSyncer(
    FixtureSyncDbContext dbContext,
    IFixtureProvider fixtureProvider,
    TimeProvider timeProvider,
    ILogger<FixtureSyncer> logger
)
{
    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Syncing fixtures from {Source}", fixtureProvider.Source);

        var items = await fixtureProvider.GetFixturesAsync(cancellationToken);

        // An empty result is far more likely to be a broken provider than a genuinely empty
        // schedule, and would otherwise delete every upcoming fixture below.
        if (items.Count == 0)
        {
            throw new InvalidOperationException($"{fixtureProvider.Source} returned no fixtures.");
        }

        var externalIds = items.Select(i => i.ExternalId).ToList();

        var fixtures = await dbContext
            .Fixtures.Where(f =>
                f.Source == fixtureProvider.Source && externalIds.Contains(f.ExternalId)
            )
            .ToDictionaryAsync(f => f.ExternalId, cancellationToken);

        foreach (var item in items)
        {
            if (!fixtures.TryGetValue(item.ExternalId, out var fixture))
            {
                fixture = new Fixture
                {
                    Source = fixtureProvider.Source,
                    ExternalId = item.ExternalId,
                };
                dbContext.Fixtures.Add(fixture);
                fixtures.Add(item.ExternalId, fixture);
            }

            fixture.Competition = item.Competition;
            fixture.HomeTeam = item.HomeTeam;
            fixture.AwayTeam = item.AwayTeam;
            fixture.KickoffUtc = item.KickoffUtc;
            fixture.Channels =
            [
                .. item.Channels.OrderBy(c => c.SortOrder).ThenBy(c => c.Name).Select(c => c.Name),
            ];
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Upcoming fixtures the provider no longer lists have been moved or dropped. A moved
        // fixture comes back under a new external id, so the old row would be a duplicate.
        var now = timeProvider.GetUtcNow();

        var removed = await dbContext
            .Fixtures.Where(f =>
                f.Source == fixtureProvider.Source
                && f.KickoffUtc > now
                && !externalIds.Contains(f.ExternalId)
            )
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation(
            "Synced {Count} fixtures and removed {Removed} no longer listed",
            items.Count,
            removed
        );
    }
}
