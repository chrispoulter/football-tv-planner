namespace FootballTvPlanner.FixtureSync.Providers;

public interface IFixtureProvider
{

    Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        CancellationToken cancellationToken = default
    );
}
