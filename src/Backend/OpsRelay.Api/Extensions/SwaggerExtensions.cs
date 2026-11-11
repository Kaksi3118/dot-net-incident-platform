using Microsoft.OpenApi.Models;
using OpsRelay.Api.Filters;

namespace OpsRelay.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "OpsRelay API",
                    Version = "v1",
                    Description =
                        "Enterprise Incident Lifecycle & SLA Management Platform API (.NET 6 Baseline)"
                });

            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, "OpsRelay.Api.xml");
            if (File.Exists(apiXmlPath))
            {
                options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
            }

            var applicationXmlPath =
                Path.Combine(AppContext.BaseDirectory, "OpsRelay.Application.xml");
            if (File.Exists(applicationXmlPath))
            {
                options.IncludeXmlComments(applicationXmlPath);
            }

            options.SchemaFilter<SwaggerEnumSchemaFilter>();
            options.OperationFilter<SwaggerExampleFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "OpsRelay API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
