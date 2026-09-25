using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Fixtures;

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

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SoccerTvDbContext>();

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

            fixture.Competition = item.Competition.Name;
            fixture.CompetitionSortOrder = item.Competition.SortOrder;
            fixture.HomeTeam = item.HomeTeam;
            fixture.AwayTeam = item.AwayTeam;
            fixture.KickoffUtc = item.KickoffUtc;
            fixture.Status = item.Status;
            fixture.Channels =
            [
                .. item.Channels.OrderBy(c => c.SortOrder).ThenBy(c => c.Name).Select(c => c.Name),
            ];
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Synced {Count} fixtures", items.Count);
    }
}
