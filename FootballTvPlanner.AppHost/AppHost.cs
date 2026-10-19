var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", databaseName: "football_tv_planner");

var mailpit = builder
    .AddMailPit("mail", httpPort: 8025, smtpPort: 1025)
    .WithLifetime(ContainerLifetime.Persistent);

var migrations = builder
    .AddProject<Projects.FootballTvPlanner_MigrationService>("migrations")
    .WithReference(database)
    .WaitFor(database);

builder
    .AddProject<Projects.FootballTvPlanner_FixtureSync>("fixture-sync")
    .WithReference(database)
    .WaitForCompletion(migrations)
    .WithExplicitStart();

var api = builder
    .AddProject<Projects.FootballTvPlanner_Api>("api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WaitForCompletion(migrations);

var web = builder
    .AddViteApp("web", "../FootballTvPlanner.Web")
    .WithEndpoint("http", e => e.Port = 5173)
    .WithEnvironment("API_URL", api.GetEndpoint("http"))
    .WaitFor(api);

api.WithEnvironment("Email__SiteUrl", web.GetEndpoint("http"));

builder.Build().Run();
