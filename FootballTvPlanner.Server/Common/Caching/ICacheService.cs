namespace FootballTvPlanner.Server.Common.Caching;

public interface ICacheService
{
    ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        string? policyName = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    );
}
