namespace Miastro.Graphics.Geometry;

public static class NatalWheelCoordinates
{
    public const double AscendantScreenAngleDegrees =
        180.0;

    public static double NormalizeDegrees(
        double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees));
        }

        var normalized =
            degrees % 360.0;

        if (normalized < 0.0)
        {
            normalized += 360.0;
        }

        return normalized;
    }

    public static double EclipticToScreenAngleDegrees(
        double eclipticLongitudeDegrees,
        double ascendantLongitudeDegrees)
    {
        var longitude =
            NormalizeDegrees(
                eclipticLongitudeDegrees);

        var ascendant =
            NormalizeDegrees(
                ascendantLongitudeDegrees);

        var relative =
            NormalizeDegrees(
                longitude - ascendant);

        return NormalizeDegrees(
            AscendantScreenAngleDegrees
            + relative);
    }

    public static ChartPoint PointOnCircle(
        ChartPoint center,
        double radius,
        double screenAngleDegrees)
    {
        if (!double.IsFinite(radius)
            || radius < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius));
        }

        var radians =
            NormalizeDegrees(
                screenAngleDegrees)
            * Math.PI
            / 180.0;

        return new ChartPoint(
            center.X
                + radius * Math.Cos(radians),
            center.Y
                - radius * Math.Sin(radians));
    }

    public static ChartPoint PointForLongitude(
        ChartPoint center,
        double radius,
        double eclipticLongitudeDegrees,
        double ascendantLongitudeDegrees)
        =>
            PointOnCircle(
                center,
                radius,
                EclipticToScreenAngleDegrees(
                    eclipticLongitudeDegrees,
                    ascendantLongitudeDegrees));
}
