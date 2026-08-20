namespace Miastro.Domain.Angles;

public readonly record struct AngularSeparation
{
    public double Degrees { get; }

    private AngularSeparation(double degrees)
    {
        if (degrees < 0.0 || degrees > 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees));
        }

        Degrees = degrees;
    }

    public static AngularSeparation Between(
        EclipticLongitude first,
        EclipticLongitude second)
    {
        var difference =
            Math.Abs(first.Degrees - second.Degrees);

        var minimum = Math.Min(
            difference,
            360.0 - difference);

        return new AngularSeparation(minimum);
    }
}
