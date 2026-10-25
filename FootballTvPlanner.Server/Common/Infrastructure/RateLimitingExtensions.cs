using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class RateLimitPolicies
{
    public const string Default = "Default";

    public const string Auth = "Auth";
}

public static class RateLimitingExtensions
{
    public static IHostApplicationBuilder AddRateLimiting(this IHostApplicationBuilder builder)
    {
        var settings =
            builder
                .Configuration.GetSection(RateLimitingSettings.SectionName)
                .Get<RateLimitingSettings>()
            ?? new RateLimitingSettings();

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.Default, CreatePolicy(settings.Default));
            options.AddPolicy(RateLimitPolicies.Auth, CreatePolicy(settings.Auth));

            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = (
                        (int)retryAfter.TotalSeconds
                    ).ToString(CultureInfo.InvariantCulture);
                }

                var problemDetailsService =
                    context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

                await problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails =
                        {
                            Status = StatusCodes.Status429TooManyRequests,
                            Title = "Too many requests. Please try again shortly.",
                        },
                    }
                );
            };
        });

        return builder;
    }

    private static Func<HttpContext, RateLimitPartition<string>> CreatePolicy(
        RateLimitPolicySettings settings
    ) =>
        httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                GetPartitionKey(httpContext),
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = settings.PermitLimit,
                    Window = settings.Window,
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                }
            );

    private static string GetPartitionKey(HttpContext httpContext) =>
        httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
            ? $"user:{userId}"
            : $"ip:{httpContext.Connection.RemoteIpAddress}";
}
