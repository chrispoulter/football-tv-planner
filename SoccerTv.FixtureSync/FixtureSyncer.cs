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
    private static readonly TimeSpan SchemaWaitTimeout = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan SchemaPollInterval = TimeSpan.FromSeconds(2);

    private readonly FixtureSyncSettings _settings = settings.Value;

    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        await WaitForSchemaAsync(cancellationToken);

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

        logger.LogInformation("Synced {Count} fixtures", items.Count);
    }

    /// <summary>
    /// The API creates the schema through its migrations, which may still be running when
    /// the sync starts (e.g. both launched together by the AppHost), so wait for the table.
    /// </summary>
    private async Task WaitForSchemaAsync(CancellationToken cancellationToken)
    {
        var deadline = timeProvider.GetUtcNow() + SchemaWaitTimeout;

        while (true)
        {
            try
            {
                // to_regclass returns null rather than erroring while the table is missing.
                var exists = await dbContext
                    .Database.SqlQuery<bool>(
                        $"SELECT to_regclass('fixtures') IS NOT NULL AS \"Value\""
                    )
                    .SingleAsync(cancellationToken);

                if (exists)
                {
                    return;
                }

                if (timeProvider.GetUtcNow() >= deadline)
                {
                    throw new TimeoutException(
                        "The fixtures table was not created in time. Has the API run its migrations?"
                    );
                }

                logger.LogInformation("Waiting for the API to create the fixtures table");
            }
            catch (Exception ex)
                when (ex is not OperationCanceledException and not TimeoutException
                    && timeProvider.GetUtcNow() < deadline
                )
            {
                logger.LogInformation(ex, "Waiting for the database to become available");
            }

            await Task.Delay(SchemaPollInterval, timeProvider, cancellationToken);
        }
    }
}
