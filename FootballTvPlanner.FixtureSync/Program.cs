using System.Diagnostics;
using System.Reflection;
using FootballTvPlanner.Data;
using FootballTvPlanner.FixtureSync;

var serviceVersion = Assembly.GetExecutingAssembly().GetSemVerShortSha();

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(serviceVersion);

builder.AddNpgsqlDbContext<FootballTvPlannerDbContext>(connectionName: "Database");
builder.AddFixtureProvider();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<FixtureSyncer>();

using var host = builder.Build();

await host.StartAsync();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

using var activitySource = new ActivitySource(builder.Environment.ApplicationName);

var exitCode = 0;

using (var activity = activitySource.StartActivity("Syncing fixtures"))
{
    try
    {
        using var scope = host.Services.CreateScope();
        var syncer = scope.ServiceProvider.GetRequiredService<FixtureSyncer>();

        await syncer.SyncAsync(lifetime.ApplicationStopping);
    }
    catch (Exception ex)
    {
        activity?.AddException(ex).SetStatus(ActivityStatusCode.Error);
        logger.LogError(ex, "Fixture sync failed");
        exitCode = 1;
    }
}

await host.StopAsync();

return exitCode;
