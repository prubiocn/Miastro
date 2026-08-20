namespace Miastro.Astronomy.Abstractions.Models;

public readonly record struct GeographicLocation
{
    public double LatitudeDegrees { get; }

    public double LongitudeDegrees { get; }

    public GeographicLocation(
        double latitudeDegrees,
        double longitudeDegrees)
    {
        if (!double.IsFinite(latitudeDegrees) ||
            latitudeDegrees is < -90.0 or > 90.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(latitudeDegrees));
        }

        if (!double.IsFinite(longitudeDegrees) ||
            longitudeDegrees is < -180.0 or > 180.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(longitudeDegrees));
        }

        LatitudeDegrees = latitudeDegrees;
        LongitudeDegrees = longitudeDegrees;
    }
}
