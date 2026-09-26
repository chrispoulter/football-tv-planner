using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SoccerTv.FixtureSync.Data;
using SoccerTv.FixtureSync.Providers;

namespace SoccerTv.FixtureSync;

public class FixtureSyncer(
    FixtureSyncDbContext dbContext,
    IFixtureProvider fixtureProvider,
    IOptions<FixtureSyncSettings> settings,
    TimeProvider timeProvider,
    ILogger<FixtureSyncer> logger
)
{
    private readonly FixtureSyncSettings _settings = settings.Value;

    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        // Pad by a day either side so every time zone's "today" is covered.
        var from = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddDays(-1);
        var to = from.AddDays(_settings.DaysAhead + 1);

        logger.LogInformation(
            "Syncing fixtures from {Source} for {From} to {To}",
            fixtureProvider.Source,
            from,
            to
        );

        var items = await fixtureProvider.GetFixturesAsync(from, to, cancellationToken);

        // An empty result is far more likely to be a broken provider than a genuinely empty
        // schedule, and would otherwise delete every upcoming fixture below.
        if (items.Count == 0)
        {
            throw new InvalidOperationException(
                $"{fixtureProvider.Source} returned no fixtures."
            );
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
