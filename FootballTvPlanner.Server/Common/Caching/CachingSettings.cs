namespace FootballTvPlanner.Server.Common.Caching;

public class CachingSettings
{
    public static string SectionName { get; } = "Caching";

    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

    public long SizeLimitBytes { get; set; } = 20 * 1024 * 1024;

    public Dictionary<string, CachePolicySettings> Policies { get; set; } = [];
}

public class CachePolicySettings
{
    public TimeSpan? Expiration { get; set; }

    public TimeSpan? LocalCacheExpiration { get; set; }
}
