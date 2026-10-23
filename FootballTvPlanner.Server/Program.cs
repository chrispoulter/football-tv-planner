using System.Reflection;
using FluentValidation;
using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Email;
using FootballTvPlanner.Server.Common.Infrastructure;

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
builder.AddDataProtection();
builder.AddAuthentication();
builder.AddOpenApi(serviceVersion);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithUI();
app.MapEndpoints(assembly);
app.MapClientConfig();
app.MapDefaultEndpoints();
app.MapFallbackToFile("index.html");

app.Run();
