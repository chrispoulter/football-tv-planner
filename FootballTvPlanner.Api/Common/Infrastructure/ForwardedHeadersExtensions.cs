using Microsoft.AspNetCore.HttpOverrides;

namespace FootballTvPlanner.Api.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
    public static IHostApplicationBuilder AddForwardedHeaders(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;

            options.ForwardedHostHeaderName = "X-FootballTvPlanner-Host";
            options.ForwardedProtoHeaderName = "X-FootballTvPlanner-Proto";

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
