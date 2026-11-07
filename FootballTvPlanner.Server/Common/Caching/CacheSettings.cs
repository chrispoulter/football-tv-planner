namespace FootballTvPlanner.Server.Common.Caching;

public class CacheSettings
{
    public static string SectionName { get; } = "Caching";

    public MemoryCacheSettings MemoryCache { get; set; } = new();

    public CachePolicySettings Default { get; set; } = new();

    public Dictionary<string, CachePolicySettings> Policies { get; set; } = [];
}

public class MemoryCacheSettings
{
    public long SizeLimitBytes { get; set; } = 20 * 1024 * 1024;
}

public class CachePolicySettings
{
    public TimeSpan? Expiration { get; set; }
}
