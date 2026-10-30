using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class CachingExtensions
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        var settings =
            builder.Configuration.GetSection(CachingSettings.SectionName).Get<CachingSettings>()
            ?? new CachingSettings();

        builder.Services.AddMemoryCache(options => options.SizeLimit = settings.SizeLimitBytes);

        builder.Services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = settings.Expiration,
                LocalCacheExpiration = settings.Expiration,
            };
        });

        return builder;
    }
}
