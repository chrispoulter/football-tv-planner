using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Common.Caching;

public static class CachingExtensions
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection(CachingSettings.SectionName);
        var settings = section.Get<CachingSettings>() ?? new CachingSettings();

        builder.Services.Configure<CachingSettings>(section);
        builder.Services.AddSingleton<ICacheService, CacheService>();

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
