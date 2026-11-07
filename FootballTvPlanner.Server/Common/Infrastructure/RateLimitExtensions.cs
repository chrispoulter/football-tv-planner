using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class RateLimitExtensions
{
    public static IHostApplicationBuilder AddRateLimiting(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<RateLimitSettings>()
            .Bind(builder.Configuration.GetSection(RateLimitSettings.SectionName));

        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = (
                        (int)retryAfter.TotalSeconds
                    ).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var route =
                    (context.HttpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText
                    ?? "unknown";

                context
                    .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .LogWarning(
                        "Rate limit exceeded for {RemoteIpAddress} on {Method} {Route}",
                        context.HttpContext.Connection.RemoteIpAddress,
                        context.HttpContext.Request.Method,
                        route
                    );

                var problemDetailsService =
                    context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

                await problemDetailsService.TryWriteAsync(
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

    public static IHostApplicationBuilder AddRateLimitPolicy(
        this IHostApplicationBuilder builder,
        string policyName,
        Func<HttpContext, string> partitionKey
    )
    {
        builder
            .Services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitSettings>>(
                (options, rateLimitSettings) =>
                {
                    var settings = rateLimitSettings.Value;

                    settings.Policies.TryGetValue(policyName, out var policy);

                    var limiterOptions = new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = policy?.PermitLimit ?? settings.PermitLimit,
                        Window = policy?.Window ?? settings.Window,
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                    };

                    options.AddPolicy(
                        policyName,
                        context =>
                            RateLimitPartition.GetSlidingWindowLimiter(
                                partitionKey(context),
                                _ => limiterOptions
                            )
                    );
                }
            );

        return builder;
    }
}
