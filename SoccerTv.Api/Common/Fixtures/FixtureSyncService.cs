using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Time;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Channels;
using SoccerTv.Api.Data.Competitions;
using SoccerTv.Api.Data.Fixtures;
using SoccerTv.Api.Data.Teams;

namespace SoccerTv.Api.Common.Fixtures;

public class FixtureSyncService(
    IServiceProvider serviceProvider,
    IFixtureProvider fixtureProvider,
    IOptions<FixtureProviderSettings> settings,
    TimeProvider timeProvider,
    ILogger<FixtureSyncService> logger
) : BackgroundService
{
    private readonly FixtureProviderSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await WaitForDatabaseAsync(cancellationToken);

        using var timer = new PeriodicTimer(
            TimeSpan.FromHours(_settings.SyncIntervalHours),
            timeProvider
        );

        do
        {
            try
            {
                await SyncAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "An error occurred while syncing fixtures");
            }
        } while (await timer.WaitForNextTickAsync(cancellationToken));
    }

    /// <summary>
    /// Migrations run in a separate hosted service, so wait until the schema is up to date
    /// before the first sync.
    /// </summary>
    private async Task WaitForDatabaseAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SoccerTvDbContext>();
                var pending = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

                if (!pending.Any())
                {
                    return;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogDebug(ex, "Database not ready for fixture sync");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), timeProvider, cancellationToken);
        }
    }

    private async Task SyncAsync(CancellationToken cancellationToken)
    {
        var from = UkTime.Today(timeProvider).AddDays(-1);
        var to = from.AddDays(_settings.DaysAhead + 1);

        logger.LogInformation(
            "Syncing fixtures from {Source} for {From} to {To}",
            fixtureProvider.Source,
            from,
            to
        );

        var items = await fixtureProvider.GetFixturesAsync(from, to, cancellationToken);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SoccerTvDbContext>();

        var competitions = await UpsertCompetitionsAsync(dbContext, items, cancellationToken);
        var teams = await UpsertTeamsAsync(dbContext, items, cancellationToken);
        var channels = await UpsertChannelsAsync(dbContext, items, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var externalIds = items.Select(i => i.ExternalId).ToList();

        var fixtures = await dbContext
            .Fixtures.Include(f => f.Broadcasts)
            .Where(f => f.Source == fixtureProvider.Source && externalIds.Contains(f.ExternalId))
            .ToDictionaryAsync(f => f.ExternalId, cancellationToken);

        var now = timeProvider.GetUtcNow();

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

            fixture.CompetitionId = competitions[item.Competition.Name].Id;
            fixture.HomeTeamId = teams[item.HomeTeam.Name].Id;
            fixture.AwayTeamId = teams[item.AwayTeam.Name].Id;
            fixture.KickoffUtc = item.KickoffUtc;
            fixture.Venue = item.Venue;
            fixture.Status = item.Status;
            fixture.UpdatedAt = now;

            var channelIds = item.Channels.Select(c => channels[c.Name].Id).ToHashSet();

            fixture.Broadcasts.RemoveAll(b => !channelIds.Contains(b.ChannelId));

            foreach (var channelId in channelIds)
            {
                if (!fixture.Broadcasts.Any(b => b.ChannelId == channelId))
                {
                    fixture.Broadcasts.Add(new FixtureBroadcast { ChannelId = channelId });
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Synced {Count} fixtures", items.Count);
    }

    private static async Task<Dictionary<string, Competition>> UpsertCompetitionsAsync(
        SoccerTvDbContext dbContext,
        IEnumerable<ProviderFixture> items,
        CancellationToken cancellationToken
    )
    {
        var existing = await dbContext.Competitions.ToDictionaryAsync(
            c => c.Name,
            cancellationToken
        );

        foreach (var source in items.Select(i => i.Competition).DistinctBy(c => c.Name))
        {
            if (!existing.TryGetValue(source.Name, out var competition))
            {
                competition = new Competition { Name = source.Name };
                dbContext.Competitions.Add(competition);
                existing.Add(source.Name, competition);
            }

            competition.ShortName = source.ShortName;
            competition.Country = source.Country;
            competition.SortOrder = source.SortOrder;
        }

        return existing;
    }

    private static async Task<Dictionary<string, Team>> UpsertTeamsAsync(
        SoccerTvDbContext dbContext,
        IEnumerable<ProviderFixture> items,
        CancellationToken cancellationToken
    )
    {
        var existing = await dbContext.Teams.ToDictionaryAsync(t => t.Name, cancellationToken);

        foreach (
            var source in items
                .SelectMany(i => new[] { i.HomeTeam, i.AwayTeam })
                .DistinctBy(t => t.Name)
        )
        {
            if (!existing.TryGetValue(source.Name, out var team))
            {
                team = new Team { Name = source.Name };
                dbContext.Teams.Add(team);
                existing.Add(source.Name, team);
            }

            team.ShortName = source.ShortName;
        }

        return existing;
    }

    private static async Task<Dictionary<string, Channel>> UpsertChannelsAsync(
        SoccerTvDbContext dbContext,
        IEnumerable<ProviderFixture> items,
        CancellationToken cancellationToken
    )
    {
        var existing = await dbContext.Channels.ToDictionaryAsync(c => c.Name, cancellationToken);

        foreach (var source in items.SelectMany(i => i.Channels).DistinctBy(c => c.Name))
        {
            if (!existing.TryGetValue(source.Name, out var channel))
            {
                channel = new Channel { Name = source.Name };
                dbContext.Channels.Add(channel);
                existing.Add(source.Name, channel);
            }

            channel.Provider = source.Provider;
            channel.Type = source.Type;
            channel.SortOrder = source.SortOrder;
        }

        return existing;
    }
}
