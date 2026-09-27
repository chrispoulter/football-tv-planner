using Microsoft.AspNetCore.HttpOverrides;

namespace SoccerTv.Api.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
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

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
