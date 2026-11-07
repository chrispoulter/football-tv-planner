using System.Security.Claims;
using FootballTvPlanner.Server.Common.RateLimiting;

namespace FootballTvPlanner.Server.Features;

public static class RateLimitPolicies
{
    public const string Default = "Default";
    public const string Auth = "Auth";
    public const string CalendarFeed = "CalendarFeed";

    public static IHostApplicationBuilder AddRateLimitPolicies(
        this IHostApplicationBuilder builder
    ) =>
        builder
            .AddRateLimitPolicy(Default, UserOrIpAddress)
            .AddRateLimitPolicy(Auth, UserOrIpAddress)
            .AddRateLimitPolicy(CalendarFeed, CalendarToken);

    public static string UserOrIpAddress(HttpContext context) =>
        context.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
            ? $"user:{userId}"
            : $"ip:{context.Connection.RemoteIpAddress}";

    public static string CalendarToken(HttpContext context) =>
        $"token:{context.GetRouteValue("token")}";
}
