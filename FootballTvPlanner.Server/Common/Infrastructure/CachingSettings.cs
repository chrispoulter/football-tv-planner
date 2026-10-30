namespace FootballTvPlanner.Server.Common.Infrastructure;

public class CachingSettings
{
    public static string SectionName { get; } = "Caching";

    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);

    public long SizeLimitBytes { get; set; } = 20 * 1024 * 1024;
}
