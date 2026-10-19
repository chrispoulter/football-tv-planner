using System.Reflection;
using FootballTvPlanner.Data;
using FootballTvPlanner.MigrationService;

var assembly = Assembly.GetExecutingAssembly();
var serviceVersion = assembly.GetSemVerShortSha();

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(serviceVersion);
builder.Services.AddHostedService<Worker>();

builder.AddNpgsqlDbContext<FootballTvPlannerDbContext>(connectionName: "Database");

builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
