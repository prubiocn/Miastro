namespace Miastro.Domain.Geography;

public readonly record struct IanaTimeZoneId
{
    public string Value { get; }

    public IanaTimeZoneId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "IANA time zone identifier cannot be empty.",
                nameof(value));
        }

        value = value.Trim();

        if (!value.Contains('/', StringComparison.Ordinal) ||
            value.Contains('\\', StringComparison.Ordinal) ||
            value.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException(
                "Invalid IANA time zone identifier format.",
                nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value;
}
