using Microsoft.AspNetCore.HttpOverrides;

namespace SoccerTv.Api.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
    /// <summary>
    /// The web app proxies /api to this API. It sends the public host, scheme and path prefix in
    /// custom headers so they can't be confused with the X-Forwarded-* headers set by the hosting
    /// provider's own proxy. These are needed to build external login redirect URIs.
    /// </summary>
    public static IHostApplicationBuilder AddForwardedHeaders(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedHost
                | ForwardedHeaders.XForwardedProto
                | ForwardedHeaders.XForwardedPrefix;

            options.ForwardedHostHeaderName = "X-SoccerTv-Host";
            options.ForwardedProtoHeaderName = "X-SoccerTv-Proto";
            options.ForwardedPrefixHeaderName = "X-SoccerTv-Prefix";

            // The proxy addresses aren't known in advance
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
