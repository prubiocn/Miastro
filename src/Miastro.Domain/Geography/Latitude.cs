namespace Miastro.Domain.Geography;

public readonly record struct Latitude
{
    public double Value { get; }

    public Latitude(double value)
    {
        if (!double.IsFinite(value) || value is < -90d or > 90d)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Latitude must be finite and in [-90, 90].");
        }

        Value = value;
    }

    public override string ToString() =>
        Value.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
}
