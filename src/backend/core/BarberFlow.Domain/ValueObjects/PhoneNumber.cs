namespace BarberFlow.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public PhoneNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var digits = new string([.. value.Where(IsAsciiDigit)]);

        if (!IsValid(digits))
        {
            throw new ArgumentException("The mobile phone number is invalid.", nameof(value));
        }

        Value = digits;
    }

    public string Value { get; }

    public override string ToString() => Value;

    private static bool IsAsciiDigit(char character) => character is >= '0' and <= '9';

    private static bool IsValid(string digits)
    {
        if (digits.Length is not (9 or 11))
        {
            return false;
        }

        var subscriberNumberIndex = digits.Length == 11 ? 2 : 0;

        if (digits[subscriberNumberIndex] != '9')
        {
            return false;
        }

        return digits.Length != 11 || digits[0] != '0';
    }
}
