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

var server = builder
    .AddProject<Projects.FootballTvPlanner_Server>("server")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithUrlForEndpoint("http", u => u.Url = "/scalar")
    .WithReference(database)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WaitForCompletion(migrations);

var client = builder
    .AddViteApp("client", "../FootballTvPlanner.Client")
    .WithEndpoint("http", e => e.Port = 5173)
    .WithEnvironment("API_URL", server.GetEndpoint("http"))
    .WaitFor(server);

server.WithEnvironment("Email__SiteUrl", client.GetEndpoint("http"));

builder.Build().Run();
