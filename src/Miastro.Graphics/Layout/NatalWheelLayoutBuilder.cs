using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed class NatalWheelLayoutBuilder
{
    public NatalWheelLayoutSnapshot Build(
        double width,
        double height,
        double ascendantLongitudeDegrees,
        double midheavenLongitudeDegrees,
        IReadOnlyList<double> houseCuspLongitudes)
    {
        ArgumentNullException.ThrowIfNull(
            houseCuspLongitudes);

        if (houseCuspLongitudes.Count != 12)
        {
            throw new ArgumentException(
                "Exactly 12 house cusps are required.",
                nameof(houseCuspLongitudes));
        }

        var ascendant =
            NatalWheelCoordinates.NormalizeDegrees(
                ascendantLongitudeDegrees);

        var midheaven =
            NatalWheelCoordinates.NormalizeDegrees(
                midheavenLongitudeDegrees);

        var metrics =
            NatalWheelMetrics.Create(
                width,
                height);

        var normalizedCusps =
            houseCuspLongitudes
                .Select(
                    NatalWheelCoordinates
                        .NormalizeDegrees)
                .ToArray();

        return new NatalWheelLayoutSnapshot(
            metrics,
            ascendant,
            midheaven,
            BuildZodiacSectors(
                ascendant),
            BuildDegreeTicks(
                metrics,
                ascendant),
            BuildHouseCusps(
                metrics,
                ascendant,
                normalizedCusps),
            BuildAngleAxes(
                metrics,
                ascendant,
                midheaven));
    }

    private static IReadOnlyList<ZodiacSectorLayout>
        BuildZodiacSectors(
            double ascendant)
    {
        var result =
            new ZodiacSectorLayout[12];

        for (var sign = 0; sign < 12; sign++)
        {
            var startLongitude =
                sign * 30.0;

            var centerLongitude =
                startLongitude + 15.0;

            result[sign] =
                new ZodiacSectorLayout(
                    sign,
                    startLongitude,
                    centerLongitude,
                    NatalWheelCoordinates
                        .EclipticToScreenAngleDegrees(
                            startLongitude,
                            ascendant),
                    30.0);
        }

        return result;
    }

    private static IReadOnlyList<DegreeTickLayout>
        BuildDegreeTicks(
            NatalWheelMetrics metrics,
            double ascendant)
    {
        var result =
            new DegreeTickLayout[360];

        for (var degree = 0; degree < 360; degree++)
        {
            var kind =
                degree % 10 == 0
                    ? DegreeTickKind.TenDegree
                    : degree % 5 == 0
                        ? DegreeTickKind.FiveDegree
                        : DegreeTickKind.Minor;

            var screenAngle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        degree,
                        ascendant);

            var tickLength =
                kind switch
                {
                    DegreeTickKind.TenDegree =>
                        10.0 * metrics.Scale,

                    DegreeTickKind.FiveDegree =>
                        7.0 * metrics.Scale,

                    _ =>
                        4.0 * metrics.Scale
                };

            result[degree] =
                new DegreeTickLayout(
                    degree,
                    kind,
                    screenAngle,
                    NatalWheelCoordinates
                        .PointOnCircle(
                            metrics.Center,
                            metrics.ZodiacInnerRadius,
                            screenAngle),
                    NatalWheelCoordinates
                        .PointOnCircle(
                            metrics.Center,
                            metrics.ZodiacInnerRadius
                                - tickLength,
                            screenAngle));
        }

        return result;
    }

    private static IReadOnlyList<HouseCuspLayout>
        BuildHouseCusps(
            NatalWheelMetrics metrics,
            double ascendant,
            IReadOnlyList<double> cusps)
    {
        var result =
            new HouseCuspLayout[12];

        // Los números de casa pertenecen visualmente a la
        // segunda circunferencia, junto a los planetas.
        var labelRadius =
            metrics.PlanetOrbitRadius
            - 18.0 * metrics.Scale;

        for (var index = 0; index < 12; index++)
        {
            var current =
                cusps[index];

            var next =
                cusps[
                    (index + 1) % 12];

            var forwardSpan =
                NatalWheelCoordinates
                    .NormalizeDegrees(
                        next - current);

            var centerLongitude =
                NatalWheelCoordinates
                    .NormalizeDegrees(
                        current
                        + forwardSpan / 2.0);

            var cuspAngle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        current,
                        ascendant);

            var centerAngle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        centerLongitude,
                        ascendant);

            result[index] =
                new HouseCuspLayout(
                    index + 1,
                    current,
                    cuspAngle,
                    NatalWheelCoordinates
                        .PointOnCircle(
                            metrics.Center,
                            metrics.OuterRadius,
                            cuspAngle),
                    NatalWheelCoordinates
                        .PointOnCircle(
                            metrics.Center,
                            metrics.AspectRadius,
                            cuspAngle),
                    centerLongitude,
                    centerAngle,
                    NatalWheelCoordinates
                        .PointOnCircle(
                            metrics.Center,
                            labelRadius,
                            centerAngle));
        }

        return result;
    }

    private static IReadOnlyList<AngleAxisLayout>
        BuildAngleAxes(
            NatalWheelMetrics metrics,
            double ascendant,
            double midheaven)
    {
        var descendant =
            NatalWheelCoordinates
                .NormalizeDegrees(
                    ascendant + 180.0);

        var imumCoeli =
            NatalWheelCoordinates
                .NormalizeDegrees(
                    midheaven + 180.0);

        return
        [
            CreateAxis(
                NatalAngleKind.Ascendant,
                ascendant),

            CreateAxis(
                NatalAngleKind.Descendant,
                descendant),

            CreateAxis(
                NatalAngleKind.Midheaven,
                midheaven),

            CreateAxis(
                NatalAngleKind.ImumCoeli,
                imumCoeli)
        ];

        AngleAxisLayout CreateAxis(
            NatalAngleKind kind,
            double longitude)
        {
            var angle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        longitude,
                        ascendant);

            return new AngleAxisLayout(
                kind,
                longitude,
                angle,
                NatalWheelCoordinates
                    .PointOnCircle(
                        metrics.Center,
                        metrics.OuterRadius
                            + 18.0 * metrics.Scale,
                        angle),
                NatalWheelCoordinates
                    .PointOnCircle(
                        metrics.Center,
                        metrics.AspectRadius,
                        angle));
        }
    }
}
