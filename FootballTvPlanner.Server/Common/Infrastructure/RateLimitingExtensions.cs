using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class RateLimitPolicies
{
    public const string Default = "Default";
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

            options.AddPolicy(
                RateLimitPolicies.Default,
                context =>
                {
                    var partitionKey = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                        is { } userId
                        ? $"user:{userId}"
                        : $"ip:{context.Connection.RemoteIpAddress}";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = settings.PermitLimit,
                            Window = settings.Window,
                            SegmentsPerWindow = 6,
                            QueueLimit = 0,
                        }
                    );
                }
            );

            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = (
                        (int)retryAfter.TotalSeconds
                    ).ToString(NumberFormatInfo.InvariantInfo);
                }

                // Log the route template, not the path, as some paths carry secrets such as calendar feed tokens
                var route =
                    (context.HttpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText
                    ?? "unknown";

                context
                    .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(typeof(RateLimitingExtensions).FullName!)
                    .LogWarning(
                        "Rate limit exceeded for {Method} {Route}",
                        context.HttpContext.Request.Method,
                        route
                    );

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
}
