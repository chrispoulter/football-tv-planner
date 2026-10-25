namespace FootballTvPlanner.Server.Common.Infrastructure;

public class RateLimitingSettings
{
    public static string SectionName { get; } = "RateLimiting";

    public RateLimitPolicySettings Default { get; set; } = new() { PermitLimit = 100 };

    public RateLimitPolicySettings Auth { get; set; } = new() { PermitLimit = 10 };
}

public class RateLimitPolicySettings
{
    public int PermitLimit { get; set; }

    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}
