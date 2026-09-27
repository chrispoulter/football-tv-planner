using System.Reflection;
using SoccerTv.FixtureSync;
using SoccerTv.FixtureSync.Data;

var serviceVersion = Assembly.GetExecutingAssembly().GetSemVerShortSha();

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(serviceVersion);

builder.AddNpgsqlDbContext<FixtureSyncDbContext>(connectionName: "Database");
builder.AddFixtureProvider();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<FixtureSyncer>();

using var host = builder.Build();

await host.StartAsync();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

var exitCode = 0;

try
{
    using var scope = host.Services.CreateScope();
    var syncer = scope.ServiceProvider.GetRequiredService<FixtureSyncer>();

    await syncer.SyncAsync(lifetime.ApplicationStopping);
}
catch (Exception ex)
{
    logger.LogError(ex, "Fixture sync failed");
    exitCode = 1;
}

await host.StopAsync();

return exitCode;
