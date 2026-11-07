using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Caching;

public class CacheService : ICacheService
{
    private readonly HybridCache _cache;

    private readonly Dictionary<string, HybridCacheEntryOptions> _entryOptions;

    public CacheService(HybridCache cache, IOptions<CacheSettings> cacheSettings)
    {
        _cache = cache;

        _entryOptions = cacheSettings.Value.Policies.ToDictionary(
            policy => policy.Key,
            policy =>
            {
                return new HybridCacheEntryOptions
                {
                    Expiration = policy.Value.Expiration ?? cacheSettings.Value.Default.Expiration,
                };
            },
            StringComparer.OrdinalIgnoreCase
        );
    }

    public ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        string? policyName = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    ) =>
        _cache.GetOrCreateAsync(
            key,
            factory,
            options: policyName is null ? null : _entryOptions.GetValueOrDefault(policyName),
            tags,
            cancellationToken
        );
}
