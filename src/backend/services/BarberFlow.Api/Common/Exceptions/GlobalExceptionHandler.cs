using BarberFlow.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Common.Exceptions;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            NotFoundException notFoundException => CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Not Found",
                notFoundException.Message),
            BadRequestException badRequestException => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                badRequestException.Message,
                new Dictionary<string, object?>
                {
                    ["errors"] = badRequestException.Errors
                }),
            ValidationException validationException => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "One or more validation errors occurred.",
                new Dictionary<string, object?>
                {
                    ["errors"] = validationException.Errors.Select(error =>
                        new
                        {
                            error.PropertyName,
                            error.ErrorMessage
                        })
                }),
            DomainException domainException => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                domainException.Message),
            _ => CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Server Error")
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        logger.LogError(
            exception,
            "Unhandled exception {ExceptionType} handled with status {StatusCode} for {Method} {Path}. TraceId: {TraceId}. CorrelationId: {CorrelationId}",
            exception.GetType().Name,
            problemDetails.Status,
            httpContext.Request.Method,
            httpContext.Request.Path.Value,
            httpContext.TraceIdentifier,
            GetCorrelationId(httpContext));

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        int status,
        string title,
        string? detail = null,
        IDictionary<string, object?>? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = status,
            Type = GetProblemType(status)
        };

        if (extensions is not null)
        {
            foreach (var extension in extensions)
            {
                problemDetails.Extensions[extension.Key] = extension.Value;
            }
        }

        return problemDetails;
    }

    private static string GetProblemType(int status)
    {
        return status switch
        {
            StatusCodes.Status400BadRequest =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            StatusCodes.Status404NotFound =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        var correlationId = httpContext.Request
            .Headers[CorrelationIdHeaderName]
            .FirstOrDefault();

        return string.IsNullOrWhiteSpace(correlationId)
            ? httpContext.TraceIdentifier
            : correlationId;
    }
}
