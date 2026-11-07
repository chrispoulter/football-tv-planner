using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Caching;

public class CachePolicyProvider : ICachePolicyProvider
{
    private readonly Dictionary<string, HybridCacheEntryOptions> _entryOptions;

    public CachePolicyProvider(IOptions<CachingSettings> cachingSettings)
    {
        var settings = cachingSettings.Value;

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

    public HybridCacheEntryOptions? Get(string policyName) =>
        _entryOptions.GetValueOrDefault(policyName);
}
