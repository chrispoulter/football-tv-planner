using System.Security.Claims;
using System.Threading.RateLimiting;

namespace FootballTvPlanner.Server.Features;

public static class DefaultRateLimit
{
    public static string Name = "Default";

    public static Func<HttpContext, RateLimitPartition<string>> Partition = context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
                ? $"user:{userId}"
                : $"ip:{context.Connection.RemoteIpAddress}",
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
            }
        );
}

public static class AuthRateLimit
{
    public static string Name = "Auth";

    public static Func<HttpContext, RateLimitPartition<string>> Partition = context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
                ? $"user:{userId}"
                : $"ip:{context.Connection.RemoteIpAddress}",
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
            }
        );
}

public static class CalendarFeedRateLimit
{
    public static string Name = "CalendarFeed";

    public static Func<HttpContext, RateLimitPartition<string>> Partition = context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: $"token:{context.GetRouteValue("token")}",
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
            }
        );
}
