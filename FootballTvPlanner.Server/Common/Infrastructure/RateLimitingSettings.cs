namespace FootballTvPlanner.Server.Common.Infrastructure;

public class RateLimitingSettings
{
    public static string SectionName { get; } = "RateLimiting";

    public int PermitLimit { get; set; } = 100;

    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}
