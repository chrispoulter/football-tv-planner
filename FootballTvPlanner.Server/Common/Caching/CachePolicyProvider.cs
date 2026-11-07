using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Caching;

public class CachePolicyProvider(IOptions<CachingSettings> cachingSettings)
    : ICachePolicyProvider
{
    private readonly Dictionary<string, HybridCacheEntryOptions> _entryOptions =
        cachingSettings.Value.Policies.ToDictionary(
            policy => policy.Key,
            policy => Build(policy.Value, cachingSettings.Value.Expiration),
            StringComparer.OrdinalIgnoreCase
        );

    public HybridCacheEntryOptions? Get(string policyName) =>
        _entryOptions.GetValueOrDefault(policyName);

    private static HybridCacheEntryOptions Build(
        CachePolicySettings policy,
        TimeSpan defaultExpiration
    )
    {
        var expiration = policy.Expiration ?? defaultExpiration;

        return new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = policy.LocalCacheExpiration ?? expiration,
        };
    }
}
