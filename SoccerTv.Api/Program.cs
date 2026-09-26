using System.Reflection;
using FluentValidation;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Database;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Users;
using SoccerTv.Api.Features;

var assembly = Assembly.GetExecutingAssembly();
var serviceVersion = assembly.GetSemVerShortSha();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(serviceVersion);

builder.AddNpgsqlDbContext<SoccerTvDbContext>(connectionName: "Database");
builder.AddEmailServices(connectionName: "Mail");

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<SoccerTvDbContext, SoccerTvDbSeeder>();

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();

builder.ConfigureJsonOptions();
builder.AddForwardedHeaders();
builder.AddAuthentication();
builder.AddOpenApi(serviceVersion);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithUI();
app.MapGroup("/account").MapIdentityApi<User>().WithTags(Tags.Account);
app.MapEndpoints(assembly);
app.MapDefaultEndpoints();

app.Run();
