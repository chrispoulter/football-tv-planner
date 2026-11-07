using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Caching;

public static class CacheExtensions
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<CacheSettings>()
            .Bind(builder.Configuration.GetSection(CacheSettings.SectionName));

        builder.Services.AddMemoryCache();
        builder
            .Services.AddOptions<MemoryCacheOptions>()
            .Configure<IOptions<CacheSettings>>(
                (options, settings) => options.SizeLimit = settings.Value.SizeLimitBytes
            );

        builder.Services.AddHybridCache();
        builder
            .Services.AddOptions<HybridCacheOptions>()
            .Configure<IOptions<CacheSettings>>(
                (options, settings) =>
                    options.DefaultEntryOptions = new HybridCacheEntryOptions
                    {
                        Expiration = settings.Value.Expiration,
                        LocalCacheExpiration = settings.Value.Expiration,
                    }
            );

        builder.Services.AddSingleton<ICacheService, CacheService>();

        return builder;
    }
}
