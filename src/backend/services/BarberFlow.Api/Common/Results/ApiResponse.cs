using BarberFlow.Domain.Results;

namespace BarberFlow.Api.Common.Results;

public sealed record ApiResponse(
    object? Value,
    bool IsFailure,
    bool IsSuccess,
    IReadOnlyCollection<Error> Errors)
{
    public static ApiResponse From(Result result) =>
        new(null, result.IsFailure, result.IsSuccess, result.Errors);
}

public sealed record ApiResponse<TValue>(
    TValue? Value,
    bool IsFailure,
    bool IsSuccess,
    IReadOnlyCollection<Error> Errors)
{
    public static ApiResponse<TValue> From(Result<TValue> result) =>
        new(
            result.IsSuccess ? result.Value : default,
            result.IsFailure,
            result.IsSuccess,
            result.Errors);
}
