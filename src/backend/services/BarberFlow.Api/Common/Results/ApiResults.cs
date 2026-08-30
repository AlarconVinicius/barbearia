using BarberFlow.Domain.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BarberFlow.Api.Common.Results;

public static class ApiResults
{
    public static IResult Problem(this ModelStateDictionary modelState)
    {
        var errors = modelState.Values
            .SelectMany(modelStateEntry => modelStateEntry.Errors)
            .Select(error => new { message = error.ErrorMessage })
            .ToArray();

        return Microsoft.AspNetCore.Http.Results.Problem(
            statusCode: GetStatusCode(ErrorType.Validation),
            title: GetTitle(ErrorType.Validation),
            type: GetType(ErrorType.Validation),
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = errors
            });
    }

    public static IResult Problem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException(
                "Cannot generate ProblemDetails for a successful result.");
        }

        var firstError = result.Errors.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "The result failed but does not contain errors.");

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: GetTitle(firstError.Type),
            detail: firstError.Message,
            statusCode: GetStatusCode(firstError.Type),
            type: GetType(firstError.Type),
            extensions: GetErrors(result));
    }

    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Unauthorized => "Unauthorized",
            _ => "Server Error"
        };
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Forbidden =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            ErrorType.Unauthorized =>
                "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static Dictionary<string, object?> GetErrors(Result result)
    {
        return new Dictionary<string, object?>
        {
            ["errors"] = result.Errors.Select(error => new
            {
                error.Code,
                error.Message
            }).ToArray()
        };
    }
}
