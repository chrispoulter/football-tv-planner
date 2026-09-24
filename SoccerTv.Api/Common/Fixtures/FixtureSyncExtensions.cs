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

            default:
                throw new InvalidOperationException(
                    $"Unknown fixture provider '{fixtureProviderSettings.Provider}'."
                );
        }

        builder.Services.AddHostedService<FixtureSyncService>();

        return builder;
    }
}
