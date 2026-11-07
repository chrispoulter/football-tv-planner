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

        var settings = cacheSettings.Value;

        _entryOptions = settings.Policies.ToDictionary(
            policy => policy.Key,
            policy =>
            {
                var expiration = policy.Value.Expiration ?? settings.Expiration;

                return new HybridCacheEntryOptions
                {
                    Expiration = expiration,
                    LocalCacheExpiration = policy.Value.LocalCacheExpiration ?? expiration,
                };
            },
            StringComparer.OrdinalIgnoreCase
        );
    }

    public ValueTask<T> GetOrCreateAsync<T>(
        string key,
        string policyName,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default
    ) =>
        _cache.GetOrCreateAsync(
            key,
            factory,
            _entryOptions.GetValueOrDefault(policyName),
            tags,
            cancellationToken
        );
}
