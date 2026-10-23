using FootballTvPlanner.FixtureSync.Providers;
using FootballTvPlanner.FixtureSync.Providers.Mock;
using FootballTvPlanner.FixtureSync.Providers.Web;

namespace FootballTvPlanner.FixtureSync;

public static class FixtureProviderExtensions
{
    public static IHostApplicationBuilder AddFixtureProvider(this IHostApplicationBuilder builder)
    {
        var fixtureSyncSettings =
            builder
                .Configuration.GetSection(FixtureSyncSettings.SectionName)
                .Get<FixtureSyncSettings>()
            ?? new FixtureSyncSettings();

        switch (fixtureSyncSettings.Provider)
        {
            case "Mock":
                builder
                    .Services.AddOptions<MockSettings>()
                    .Bind(builder.Configuration.GetSection(MockSettings.SectionName));
                builder.Services.AddSingleton<IFixtureProvider, MockFixtureProvider>();
                break;

            case "Web":
                builder
                    .Services.AddOptions<WebSettings>()
                    .Bind(builder.Configuration.GetSection(WebSettings.SectionName));
                builder.Services.AddHttpClient(WebFixtureProvider.HttpClientName);
                builder.Services.AddSingleton<IFixtureProvider, WebFixtureProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown fixture provider '{fixtureSyncSettings.Provider}'."
                );
        }

        return builder;
    }
}
