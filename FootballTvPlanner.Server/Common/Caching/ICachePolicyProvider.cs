using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Common.Caching;

public interface ICachePolicyProvider
{
    HybridCacheEntryOptions? Get(string policyName);
}
