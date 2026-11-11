using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpsRelay.Api.Filters;

public class SwaggerExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath ?? string.Empty;
        var method = context.ApiDescription.HttpMethod ?? string.Empty;

        if (HttpMethods.IsPost(method)
            && path.Equals("api/v1/incidents", StringComparison.OrdinalIgnoreCase))
        {
            ApplyCreateIncidentExample(operation);
        }

        if (HttpMethods.IsPatch(method)
            && path.Contains("status", StringComparison.OrdinalIgnoreCase))
        {
            ApplyUpdateStatusExample(operation);
        }

        AddProblemDetailsExamples(operation);
    }

    private static void ApplyCreateIncidentExample(OpenApiOperation operation)
    {
        if (!operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
        {
            return;
        }

        mediaType.Example = new OpenApiObject
        {
            ["title"] = new OpenApiString("Database connection pool exhausted"),
            ["description"] = new OpenApiString(
                "The primary SQL Server instance is rejecting new connections."),
            ["priority"] = new OpenApiString("Critical"),
            ["assignedTeamId"] = new OpenApiString("SRE-Ops")
        };
    }

    private static void ApplyUpdateStatusExample(OpenApiOperation operation)
    {
        if (!operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
        {
            return;
        }

        mediaType.Example = new OpenApiObject
        {
            ["newStatus"] = new OpenApiString("Triaged"),
            ["reason"] = new OpenApiString("Escalated to the on-call engineer.")
        };
    }

    private static void AddProblemDetailsExamples(OpenApiOperation operation)
    {
        if (operation.Responses.TryGetValue("400", out var badRequest))
        {
            var example = BuildProblemDetailsResponse(
                "Validation error",
                "One or more validation errors occurred.",
                400,
                includeErrors: true);

            badRequest.Description = "Validation error (RFC 7807 ProblemDetails)";
            badRequest.Content = example.Content;
        }

        if (operation.Responses.TryGetValue("404", out var notFound))
        {
            var example = BuildProblemDetailsResponse(
                "Resource not found",
                "The requested resource was not found.",
                404,
                includeErrors: false);

            notFound.Description = "Resource not found (RFC 7807 ProblemDetails)";
            notFound.Content = example.Content;
        }
    }

    private static OpenApiResponse BuildProblemDetailsResponse(
        string description,
        string detail,
        int status,
        bool includeErrors)
    {
        var example = new OpenApiObject
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7807"),
            ["title"] = new OpenApiString(description),
            ["status"] = new OpenApiInteger(status),
            ["detail"] = new OpenApiString(detail),
            ["instance"] = new OpenApiString("/api/v1/incidents")
        };

        if (includeErrors)
        {
            example["errors"] = new OpenApiObject
            {
                ["Title"] = new OpenApiArray
                {
                    new OpenApiString("'Title' must be between 5 and 200 characters.")
                }
            };
        }

        return new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Example = example
                }
            }
        };
    }
}
