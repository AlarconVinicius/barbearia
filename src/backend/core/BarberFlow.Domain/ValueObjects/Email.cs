using System.Net.Mail;

namespace BarberFlow.Domain.ValueObjects;

public sealed record Email
{
    public Email(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalizedValue, out var address) ||
            !string.Equals(address.Address, normalizedValue, StringComparison.Ordinal))
        {
            throw new ArgumentException("The email address is invalid.", nameof(value));
        }

        Value = normalizedValue;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
