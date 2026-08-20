namespace Miastro.Domain.Angles;

public readonly record struct EclipticLongitude : IComparable<EclipticLongitude>
{
    public double Degrees { get; }

    private EclipticLongitude(double degrees)
    {
        Degrees = Angle.NormalizeDegrees(degrees);
    }

    public static EclipticLongitude FromDegrees(double degrees) =>
        new(degrees);

    public int CompareTo(EclipticLongitude other) =>
        Degrees.CompareTo(other.Degrees);

    public static EclipticLongitude operator +(
        EclipticLongitude longitude,
        Angle angle) =>
        new(longitude.Degrees + angle.Degrees);

    public static EclipticLongitude operator -(
        EclipticLongitude longitude,
        Angle angle) =>
        new(longitude.Degrees - angle.Degrees);
}
