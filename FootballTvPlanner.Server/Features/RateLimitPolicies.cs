using System.Security.Claims;

namespace FootballTvPlanner.Server.Features;

public static class DefaultRateLimit
{
    public const string Name = "Default";

    public static string PartitionKey(HttpContext context) =>
        RateLimitPartitionKeys.UserOrIpAddress(context);
}

public static class AuthRateLimit
{
    public const string Name = "Auth";

    public static string PartitionKey(HttpContext context) =>
        RateLimitPartitionKeys.UserOrIpAddress(context);
}

public static class CalendarFeedRateLimit
{
    public const string Name = "CalendarFeed";

    public static string PartitionKey(HttpContext context) =>
        $"token:{context.GetRouteValue("token")}";
}

internal static class RateLimitPartitionKeys
{
    public static string UserOrIpAddress(HttpContext context) =>
        context.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
            ? $"user:{userId}"
            : $"ip:{context.Connection.RemoteIpAddress}";
}
