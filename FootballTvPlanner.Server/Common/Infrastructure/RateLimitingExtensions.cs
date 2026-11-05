using System.Globalization;
using System.Threading.RateLimiting;

namespace FootballTvPlanner.Server.Common.Infrastructure;

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
}
