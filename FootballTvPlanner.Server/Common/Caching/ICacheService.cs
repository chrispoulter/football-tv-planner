namespace FootballTvPlanner.Server.Common.Caching;

public interface ICacheService
{
    ValueTask<T> GetOrCreateAsync<T>(
        string key,
        string policyName,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    );
}
