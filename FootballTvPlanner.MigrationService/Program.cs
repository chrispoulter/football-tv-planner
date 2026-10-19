using System.Diagnostics;
using System.Reflection;
using FootballTvPlanner.Data;
using Microsoft.EntityFrameworkCore;

const string activitySourceName = "Migrations";

var serviceVersion = Assembly.GetExecutingAssembly().GetSemVerShortSha();

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(serviceVersion);

builder.AddNpgsqlDbContext<FootballTvPlannerDbContext>(connectionName: "Database");

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(activitySourceName));

using var host = builder.Build();

await host.StartAsync();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

using var activitySource = new ActivitySource(activitySourceName);

var exitCode = 0;

using (var activity = activitySource.StartActivity("Migrating database", ActivityKind.Client))
{
    try
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FootballTvPlannerDbContext>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            lifetime.ApplicationStopping,
            dbContext.Database.MigrateAsync
        );
    }
    catch (Exception ex)
    {
        activity?.AddException(ex);
        logger.LogError(ex, "Database migration failed");
        exitCode = 1;
    }
}

await host.StopAsync();

return exitCode;
