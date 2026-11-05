using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features;

public static class CachePolicies
{
    public static HybridCacheEntryOptions Default = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };
}
