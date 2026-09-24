namespace SoccerTv.Api.Common.Fixtures;

public interface IFixtureProvider
{
    /// <summary>
    /// Identifies the provider. Stored against each fixture so that external IDs from
    /// different providers never collide.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Returns the televised fixtures kicking off between <paramref name="from"/> and
    /// <paramref name="to"/> (inclusive, UK dates).
    /// </summary>
    Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    );
}
