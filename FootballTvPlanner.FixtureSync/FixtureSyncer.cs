using FootballTvPlanner.Data;
using FootballTvPlanner.Data.Channels;
using FootballTvPlanner.Data.Competitions;
using FootballTvPlanner.Data.Fixtures;
using FootballTvPlanner.Data.Teams;
using FootballTvPlanner.FixtureSync.Providers;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.FixtureSync;

public class FixtureSyncer(
    FootballTvPlannerDbContext dbContext,
    IFixtureProvider fixtureProvider,
    TimeProvider timeProvider,
    ILogger<FixtureSyncer> logger
)
{
    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Syncing fixtures from {Source}", fixtureProvider.Source);

        var items = await fixtureProvider.GetFixturesAsync(cancellationToken);

        if (items.Count == 0)
        {
            throw new InvalidOperationException($"{fixtureProvider.Source} returned no fixtures.");
        }

        // The sync owns every row, so it must see hidden competitions, channels and the fixtures
        // they hide; otherwise it would re-insert them and break the unique indexes.
        var competitions = await dbContext
            .Competitions.IgnoreQueryFilters()
            .ToDictionaryAsync(c => c.Name, cancellationToken);
        var teams = await dbContext.Teams.ToDictionaryAsync(t => t.Name, cancellationToken);
        var channels = await dbContext
            .Channels.IgnoreQueryFilters()
            .ToDictionaryAsync(c => c.Name, cancellationToken);

        var externalIds = items.Select(i => i.ExternalId).ToList();

        var fixtures = await dbContext
            .Fixtures.IgnoreQueryFilters()
            .Include(f => f.Channels)
            .Where(f => f.Source == fixtureProvider.Source && externalIds.Contains(f.ExternalId))
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

            fixture.Competition = GetOrAdd(
                competitions,
                item.Competition,
                name => new Competition { Name = name }
            );
            fixture.HomeTeam = GetOrAdd(teams, item.HomeTeam, name => new Team { Name = name });
            fixture.AwayTeam = GetOrAdd(teams, item.AwayTeam, name => new Team { Name = name });
            fixture.KickoffUtc = item.KickoffUtc;

            var itemChannels = item
                .Channels.Distinct()
                .Select(c => GetOrAdd(channels, c, name => new Channel { Name = name }))
                .ToList();

            fixture.Channels.RemoveAll(c => !itemChannels.Contains(c));
            fixture.Channels.AddRange(itemChannels.Except(fixture.Channels));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var now = timeProvider.GetUtcNow();

        var removed = await dbContext
            .Fixtures.IgnoreQueryFilters()
            .Where(f =>
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

    private T GetOrAdd<T>(Dictionary<string, T> existing, string name, Func<string, T> create)
        where T : class
    {
        if (!existing.TryGetValue(name, out var entity))
        {
            entity = create(name);
            dbContext.Add(entity);
            existing.Add(name, entity);
        }

        return entity;
    }
}
