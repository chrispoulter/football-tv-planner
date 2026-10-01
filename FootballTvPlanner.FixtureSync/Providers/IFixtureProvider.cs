namespace FootballTvPlanner.FixtureSync.Providers;

public interface IFixtureProvider
{
    string Source { get; }

    Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        CancellationToken cancellationToken = default
    );
}
