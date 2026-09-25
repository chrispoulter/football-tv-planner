using SoccerTv.Api.Common.Fixtures.LiveFootballOnTv;

namespace SoccerTv.Api.Common.Fixtures;

public static class FixtureSyncExtensions
{
    public static IHostApplicationBuilder AddFixtureSync(this IHostApplicationBuilder builder)
    {
        var fixtureProviderConfig = builder.Configuration.GetSection(
            FixtureProviderSettings.SectionName
        );
        builder.Services.Configure<FixtureProviderSettings>(fixtureProviderConfig);

        var fixtureProviderSettings =
            fixtureProviderConfig.Get<FixtureProviderSettings>() ?? new FixtureProviderSettings();

        switch (fixtureProviderSettings.Provider)
        {
            case "Mock":
                builder.Services.AddSingleton<IFixtureProvider, MockFixtureProvider>();
                break;

            case "LiveFootballOnTv":
                builder.Services.Configure<LiveFootballOnTvSettings>(
                    builder.Configuration.GetSection(LiveFootballOnTvSettings.SectionName)
                );
                builder.Services.AddHttpClient(
                    LiveFootballOnTvFixtureProvider.HttpClientName,
                    client =>
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("SoccerTvSchedule/1.0")
                );
                builder.Services.AddSingleton<IFixtureProvider, LiveFootballOnTvFixtureProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown fixture provider '{fixtureProviderSettings.Provider}'."
                );
        }

        builder.Services.AddHostedService<FixtureSyncService>();

        return builder;
    }
}
