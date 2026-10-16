using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

                options.AddOperationTransformer(
                    (operation, context, cancellationToken) =>
                    {
                        var hasEmptyBody = context.Description.ParameterDescriptions.Any(p =>
                            p.Source == BindingSource.Body && p.Type == typeof(object)
                        );

                        if (
                            hasEmptyBody
                            && operation.RequestBody?.Content?.TryGetValue(
                                "application/json",
                                out var mediaType
                            ) == true
                        )
                        {
                            mediaType.Schema = new OpenApiSchema { Type = JsonSchemaType.Object };

                            mediaType.Examples = new Dictionary<string, IOpenApiExample>
                            {
                                ["empty"] = new OpenApiExample { Value = new JsonObject() },
                            };
                        }

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
