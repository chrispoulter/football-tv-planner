namespace FootballTvPlanner.Server.Common.RateLimiting;

public class RateLimitSettings
{
    public static string SectionName { get; } = "RateLimiting";

    public int PermitLimit { get; set; } = 100;

    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    public Dictionary<string, RateLimitPolicySettings> Policies { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

public class RateLimitPolicySettings
{
    public int? PermitLimit { get; set; }

    public TimeSpan? Window { get; set; }
}
