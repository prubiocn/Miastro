namespace Miastro.Domain.Geography;

public readonly record struct Longitude
{
    public double Value { get; }

    public Longitude(double value)
    {
        if (!double.IsFinite(value) || value is < -180d or > 180d)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Longitude must be finite and in [-180, 180].");
        }

        Value = value;
    }

    public override string ToString() =>
        Value.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
}
