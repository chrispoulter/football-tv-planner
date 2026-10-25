using System.Globalization;
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
            options.AddPolicy(
                RateLimitPolicies.Default,
                context =>
                {
                    var partitionKey = $"ip:{context.Connection.RemoteIpAddress}";

                    if (context.User.Identity?.IsAuthenticated is true)
                    {
                        partitionKey = $"user:{context.User.Identity.Name}";
                    }

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

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context
                    .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .LogWarning(
                        "Rate limit exceeded for {Method} {Path}",
                        context.HttpContext.Request.Method,
                        context.HttpContext.Request.Path
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
