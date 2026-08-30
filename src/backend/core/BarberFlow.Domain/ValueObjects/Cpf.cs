namespace BarberFlow.Domain.ValueObjects;

public sealed record Cpf
{
    public Cpf(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var digits = new string([.. value.Where(IsAsciiDigit)]);

        if (!IsValid(digits))
        {
            throw new ArgumentException("The CPF is invalid.", nameof(value));
        }

        Value = digits;
    }

    public string Value { get; }

    public override string ToString() => Value;

    private static bool IsValid(string digits)
    {
        if (digits.Length != 11 || digits.All(digit => digit == digits[0]))
        {
            return false;
        }

        return CalculateCheckDigit(digits, 9) == digits[9] - '0' &&
               CalculateCheckDigit(digits, 10) == digits[10] - '0';
    }

    private static int CalculateCheckDigit(string digits, int length)
    {
        var sum = 0;

        for (var index = 0; index < length; index++)
        {
            sum += (digits[index] - '0') * (length + 1 - index);
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static bool IsAsciiDigit(char character) => character is >= '0' and <= '9';
}
