using System.Reflection;
using FluentValidation;
using FootballTvPlanner.Data;
using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Caching;
using FootballTvPlanner.Server.Common.Email;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Features;

var assembly = Assembly.GetExecutingAssembly();
var serviceVersion = assembly.GetSemVerShortSha();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(serviceVersion);

builder.AddNpgsqlDbContext<FootballTvPlannerDbContext>(connectionName: "Database");
builder.AddEmailServices(connectionName: "Mail");

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();

builder.ConfigureJsonOptions();
builder.AddForwardedHeaders();
builder.AddDataProtection<FootballTvPlannerDbContext>();
builder.AddAuthentication<User, FootballTvPlannerDbContext>();
builder.AddCaching();
builder
    .AddRateLimiting()
    .AddRateLimitPolicy(DefaultRateLimit.Name, DefaultRateLimit.PartitionKey)
    .AddRateLimitPolicy(AuthRateLimit.Name, AuthRateLimit.PartitionKey)
    .AddRateLimitPolicy(CalendarFeedRateLimit.Name, CalendarFeedRateLimit.PartitionKey);

builder.AddOpenApi(serviceVersion);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseClientStaticFiles();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapOpenApiWithUI();
app.MapEndpoints(assembly).RequireRateLimiting(DefaultRateLimit.Name);
app.MapClientConfig();
app.MapDefaultEndpoints();
app.MapClientFallback();

app.Run();
