using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpsRelay.Domain.Exceptions;

namespace OpsRelay.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (exception is ValidationException)
            {
                _logger.LogWarning(
                    "Validation failed for {RequestPath}: {Message}",
                    context.Request.Path,
                    exception.Message);
            }
            else if (exception is DomainException or KeyNotFoundException)
            {
                _logger.LogWarning(
                    "Request {RequestPath} rejected: {Message}",
                    context.Request.Path,
                    exception.Message);
            }
            else
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception for {RequestPath}",
                    context.Request.Path);
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                validationException.Message),
            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                "A domain rule was violated.",
                domainException.Message),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "The request could not be completed. Please try again later.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (exception is ValidationException validationFailure)
        {
            problemDetails.Extensions["errors"] = validationFailure.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
