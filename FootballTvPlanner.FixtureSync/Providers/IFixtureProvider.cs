namespace FootballTvPlanner.FixtureSync.Providers;

public interface IFixtureProvider
{
    /// <summary>
    /// Identifies the provider. Stored against each fixture so that external IDs from
    /// different providers never collide.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Returns the upcoming televised fixtures. Each provider decides how far ahead to look.
    /// </summary>
    Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        CancellationToken cancellationToken = default
    );
}
