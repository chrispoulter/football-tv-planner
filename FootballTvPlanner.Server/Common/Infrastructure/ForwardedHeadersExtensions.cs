using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class ForwardedHeadersExtensions
{
    public static IHostApplicationBuilder AddForwardedHeaders(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<ForwardedHeadersSettings>()
            .Bind(builder.Configuration.GetSection(ForwardedHeadersSettings.SectionName));

        builder
            .Services.AddOptions<ForwardedHeadersOptions>()
            .Configure<IOptions<ForwardedHeadersSettings>>(
                (options, settings) =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor
                        | ForwardedHeaders.XForwardedHost
                        | ForwardedHeaders.XForwardedProto;

                    options.ForwardedForHeaderName = settings.Value.ForwardedForHeaderName;

                    options.KnownIPNetworks.Clear();
                    options.KnownProxies.Clear();
                }
            );

        return builder;
    }
}
