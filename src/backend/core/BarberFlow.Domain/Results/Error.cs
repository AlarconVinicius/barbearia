namespace BarberFlow.Domain.Results;

public record Error
{
    protected Error(string code, string message, ErrorType type)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Type = type;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public static readonly Error None = new(
        string.Empty,
        string.Empty,
        ErrorType.Failure);

    public static readonly Error NullValue = new(
        "NULL_VALUE",
        "Null value was provided.",
        ErrorType.Failure);

    public static readonly Error ConditionNotMet = new(
        "CONDITION_NOT_MET",
        "The specified condition was not met.",
        ErrorType.Failure);

    public static Error NotFound(string message) =>
        new("NOT_FOUND", message, ErrorType.NotFound);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Validation(string message) =>
        new("VALIDATION_ERROR", message, ErrorType.Validation);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error Conflict(string message) =>
        new("CONFLICT_ERROR", message, ErrorType.Conflict);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Failure(string message) =>
        new("INTERNAL_SERVER_ERROR", message, ErrorType.Failure);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public static Error Forbidden(string message) =>
        new("FORBIDDEN", message, ErrorType.Forbidden);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error Unauthorized(string message) =>
        new("UNAUTHORIZED", message, ErrorType.Unauthorized);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Problem(string message) =>
        new("PROBLEM", message, ErrorType.Problem);

    public static Error Problem(string code, string message) =>
        new(code, message, ErrorType.Problem);

    public override string ToString() => $"[{Type}] {Code}: {Message}";
}
