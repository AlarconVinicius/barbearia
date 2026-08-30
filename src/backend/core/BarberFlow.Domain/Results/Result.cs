using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BarberFlow.Domain.Results;

public class Result
{
    private readonly List<Error> _errors = [];

    protected Result()
    {
    }

    protected Result(IEnumerable<Error> errors)
    {
        AddErrors(errors);
    }

    [JsonConstructor]
    public Result(bool isSuccess, List<Error>? errors = null)
    {
        if (isSuccess)
        {
            if (errors is { Count: > 0 })
            {
                throw new ArgumentException(
                    "A successful result cannot contain errors.",
                    nameof(errors));
            }

            return;
        }

        AddErrors(errors);
    }

    public bool IsFailure => _errors.Count > 0;

    public bool IsSuccess => !IsFailure;

    public IReadOnlyList<Error> Errors => _errors;

    public static Result Ok() => new();

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result([error]);
    }

    public static Result Failure(IEnumerable<Error> errors) => new(errors);

    public static Result<TValue> Ok<TValue>(TValue value) => new(value);

    public static Result<TValue> Failure<TValue>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<TValue>([error]);
    }

    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors) =>
        new(errors);

    public static implicit operator Result(Error error) => Failure(error);

    public static implicit operator Result(List<Error> errors) => Failure(errors);

    private void AddErrors(IEnumerable<Error>? errors)
    {
        var errorList = errors?.ToList();

        if (errorList is not { Count: > 0 })
        {
            throw new ArgumentException(
                "Error list cannot be null or empty for a failed result.",
                nameof(errors));
        }

        _errors.AddRange(errorList);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue value)
    {
        _value = value;
    }

    protected internal Result(IEnumerable<Error> errors)
        : base(errors)
    {
    }

    [JsonConstructor]
    public Result(
        bool isSuccess,
        List<Error>? errors = null,
        TValue? value = default)
        : base(isSuccess, errors)
    {
        if (isSuccess)
        {
            _value = value;
        }
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            "Cannot access the value of a failed result.");

    public static implicit operator Result<TValue>(TValue value) => Ok(value);

    public static implicit operator Result<TValue>(Error error) =>
        Failure<TValue>(error);

    public static implicit operator Result<TValue>(List<Error> errors) =>
        Failure<TValue>(errors);
}
