using Microsoft.AspNetCore.HttpOverrides;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
    public static IHostApplicationBuilder AddForwardedHeaders(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor
                | ForwardedHeaders.XForwardedHost
                | ForwardedHeaders.XForwardedProto;

            // Cloudflare sets the original client IP here; X-Forwarded-For would yield its edge IP
            options.ForwardedForHeaderName = "CF-Connecting-IP";

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
