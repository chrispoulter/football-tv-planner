var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", databaseName: "soccer_tv_schedule");

var mailpit = builder
    .AddMailPit("mail", httpPort: 8025, smtpPort: 1025)
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder
    .AddProject<Projects.SoccerTv_Api>("api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(mailpit)
    .WaitFor(mailpit);

// Runs a single sync once the API (which owns the migrations) is up; re-run it from the
// dashboard to refresh fixtures.
builder
    .AddProject<Projects.SoccerTv_FixtureSync>("fixture-sync")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(api);

var web = builder
    .AddViteApp("web", "../SoccerTv.Web")
    .WithEndpoint("http", e => e.Port = 5173)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
    .WithReference(api)
    .WaitFor(api);

api.WithEnvironment("Email__SiteUrl", web.GetEndpoint("http"));

builder.Build().Run();
