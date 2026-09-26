using SoccerTv.FixtureSync.Providers;
using SoccerTv.FixtureSync.Providers.LiveFootballOnTv;
using SoccerTv.FixtureSync.Providers.Mock;

namespace SoccerTv.FixtureSync;

public static class FixtureProviderExtensions
{
    public static IHostApplicationBuilder AddFixtureProvider(this IHostApplicationBuilder builder)
    {
        var fixtureSyncConfig = builder.Configuration.GetSection(FixtureSyncSettings.SectionName);
        builder.Services.Configure<FixtureSyncSettings>(fixtureSyncConfig);

        var fixtureSyncSettings =
            fixtureSyncConfig.Get<FixtureSyncSettings>() ?? new FixtureSyncSettings();

        switch (fixtureSyncSettings.Provider)
        {
            case "Mock":
                builder.Services.Configure<MockSettings>(
                    builder.Configuration.GetSection(MockSettings.SectionName)
                );
                builder.Services.AddSingleton<IFixtureProvider, MockFixtureProvider>();
                break;

            case "LiveFootballOnTv":
                builder.Services.Configure<LiveFootballOnTvSettings>(
                    builder.Configuration.GetSection(LiveFootballOnTvSettings.SectionName)
                );
                builder.Services.AddHttpClient(LiveFootballOnTvFixtureProvider.HttpClientName);
                builder.Services.AddSingleton<IFixtureProvider, LiveFootballOnTvFixtureProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown fixture provider '{fixtureSyncSettings.Provider}'."
                );
        }

        return builder;
    }
}
