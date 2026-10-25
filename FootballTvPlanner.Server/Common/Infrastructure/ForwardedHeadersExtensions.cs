using Microsoft.AspNetCore.HttpOverrides;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
    public static IHostApplicationBuilder AddForwardedHeaders(this IHostApplicationBuilder builder)
    {
        var settings =
            builder
                .Configuration.GetSection(ForwardedHeadersSettings.SectionName)
                .Get<ForwardedHeadersSettings>()
            ?? new ForwardedHeadersSettings();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor
                | ForwardedHeaders.XForwardedHost
                | ForwardedHeaders.XForwardedProto;

            options.ForwardedForHeaderName = settings.ForwardedForHeaderName;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
