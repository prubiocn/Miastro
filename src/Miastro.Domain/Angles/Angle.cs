namespace Miastro.Domain.Angles;

public readonly record struct Angle : IComparable<Angle>
{
    public double Degrees { get; }

    private Angle(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees),
                "El ángulo debe ser finito.");
        }

        Degrees = degrees;
    }

    public static Angle FromDegrees(double degrees) =>
        new(degrees);

    public Angle Normalize360() =>
        new(NormalizeDegrees(Degrees));

    public static Angle operator +(Angle left, Angle right) =>
        new(left.Degrees + right.Degrees);

    public static Angle operator -(Angle left, Angle right) =>
        new(left.Degrees - right.Degrees);

    public int CompareTo(Angle other) =>
        Degrees.CompareTo(other.Degrees);

    public static double NormalizeDegrees(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees),
                "El ángulo debe ser finito.");
        }

        var normalized = degrees % 360.0;

        if (normalized < 0.0)
        {
            normalized += 360.0;
        }

        if (normalized >= 360.0)
        {
            normalized = 0.0;
        }

        return normalized;
    }
}
