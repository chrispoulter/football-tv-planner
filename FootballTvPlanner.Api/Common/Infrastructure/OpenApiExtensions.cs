using System.Reflection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace FootballTvPlanner.Api.Common.Infrastructure;

public static class OpenApiExtensions
{
    public static IHostApplicationBuilder AddOpenApi(
        this IHostApplicationBuilder builder,
        string? serviceVersion
    )
    {
        builder.Services.AddOpenApi(
            "v1",
            options =>
            {
                options.AddDocumentTransformer(
                    (document, context, cancellationToken) =>
                    {
                        document.Info ??= new OpenApiInfo();
                        document.Info.Version = serviceVersion ?? document.Info.Version;

                        return Task.CompletedTask;
                    }
                );
            }
        );

        return builder;
    }

    public static WebApplication MapOpenApiWithUI(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(
            "/",
            options =>
            {
                options.Title = app.Environment.ApplicationName;
            }
        );

        return app;
    }
}
