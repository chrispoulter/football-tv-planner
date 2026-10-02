using FootballTvPlanner.FixtureSync.Data;
using FootballTvPlanner.FixtureSync.Providers;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.FixtureSync;

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

        if (items.Count == 0)
        {
            throw new InvalidOperationException($"{fixtureProvider.Source} returned no fixtures.");
        }

        var competitionIds = await GetOrAddCompetitionsAsync(
            items.Select(i => i.Competition),
            cancellationToken
        );

        var channelIds = await GetOrAddChannelsAsync(
            items.SelectMany(i => i.Channels),
            cancellationToken
        );

        var externalIds = items.Select(i => i.ExternalId).ToList();

        var fixtures = await dbContext
            .Fixtures.Include(f => f.FixtureChannels)
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

            fixture.CompetitionId = competitionIds[item.Competition];
            fixture.HomeTeam = item.HomeTeam;
            fixture.AwayTeam = item.AwayTeam;
            fixture.KickoffUtc = item.KickoffUtc;

            SyncChannels(fixture, item.Channels, channelIds);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

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

    /// <summary>
    /// Updates the fixture's channel links in place. Removing and re-adding a link with the same
    /// key in one save makes EF Core throw, so only the differences are applied.
    /// </summary>
    private static void SyncChannels(
        Fixture fixture,
        IReadOnlyList<string> channels,
        Dictionary<string, Guid> channelIds
    )
    {
        var wanted = channels.Select(c => channelIds[c]).ToHashSet();

        fixture.FixtureChannels.RemoveAll(fc => !wanted.Contains(fc.ChannelId));

        var existing = fixture.FixtureChannels.Select(fc => fc.ChannelId).ToHashSet();

        foreach (var channelId in wanted.Where(id => !existing.Contains(id)))
        {
            fixture.FixtureChannels.Add(new FixtureChannel { ChannelId = channelId });
        }
    }

    private async Task<Dictionary<string, Guid>> GetOrAddCompetitionsAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken
    )
    {
        var ids = await dbContext.Competitions.ToDictionaryAsync(
            c => c.Name,
            c => c.Id,
            cancellationToken
        );

        foreach (var name in names.Distinct().Where(n => !ids.ContainsKey(n)))
        {
            var competition = new Competition { Id = Guid.CreateVersion7(), Name = name };
            dbContext.Competitions.Add(competition);
            ids.Add(name, competition.Id);

            logger.LogWarning("New competition discovered: {Name}", name);
        }

        return ids;
    }

    private async Task<Dictionary<string, Guid>> GetOrAddChannelsAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken
    )
    {
        var ids = await dbContext.Channels.ToDictionaryAsync(
            c => c.Name,
            c => c.Id,
            cancellationToken
        );

        foreach (var name in names.Distinct().Where(n => !ids.ContainsKey(n)))
        {
            var channel = new Channel { Id = Guid.CreateVersion7(), Name = name };
            dbContext.Channels.Add(channel);
            ids.Add(name, channel.Id);

            logger.LogWarning("New channel discovered: {Name}", name);
        }

        return ids;
    }
}
