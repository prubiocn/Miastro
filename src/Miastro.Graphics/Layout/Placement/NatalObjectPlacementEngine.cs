using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout.Placement;

public sealed class NatalObjectPlacementEngine
{
    private const double GeometryTolerance =
        1e-9;

    public NatalObjectPlacementSnapshot Layout(
        NatalWheelLayoutSnapshot wheel,
        IReadOnlyList<NatalObjectLayoutInput> objects,
        NatalGlyphLayoutPolicy? policy = null)
    {
        ArgumentNullException.ThrowIfNull(
            wheel);

        ArgumentNullException.ThrowIfNull(
            objects);

        policy ??=
            NatalGlyphLayoutPolicy.FromMetrics(
                wheel.Metrics);

        ValidatePolicy(
            policy);

        var ascendant =
            wheel.AscendantLongitudeDegrees;

        var baseRadius =
            (
                wheel.Metrics.HouseOuterRadius
                + wheel.Metrics.ZodiacInnerRadius
            )
            / 2.0;

        var protectedSize =
            policy.GlyphSize
            + policy.MinimumGap;

        var protectedHalfDiagonal =
            protectedSize
            / Math.Sqrt(2.0);

        var minimumSafeRadius =
            wheel.Metrics.HouseInnerRadius
            + protectedHalfDiagonal;

        var maximumSafeRadius =
            wheel.Metrics.ZodiacInnerRadius
            - protectedHalfDiagonal;

        if (minimumSafeRadius
            >= maximumSafeRadius)
        {
            throw new InvalidOperationException(
                "Glyph footprint does not fit the safe natal placement annulus.");
        }

        var effectiveBaseRadius =
            Math.Clamp(
                baseRadius,
                minimumSafeRadius,
                maximumSafeRadius);

        var angularStepDegrees =
            CalculateAngularStepDegrees(
                protectedSize,
                minimumSafeRadius);

        var ordered =
            objects
                .Select(
                    item =>
                        new NatalObjectLayoutInput(
                            item.Id,
                            NatalWheelCoordinates
                                .NormalizeDegrees(
                                    item.RealLongitudeDegrees)))
                .OrderBy(
                    item => item.RealLongitudeDegrees)
                .ThenBy(
                    item => item.Id,
                    StringComparer.Ordinal)
                .ToArray();

        EnsureUniqueIds(
            ordered);

        var placed =
            new List<NatalVisualPlacement>(
                ordered.Length);

        double? previousVisualUnwrapped =
            null;

        foreach (var item in ordered)
        {
            var realAngle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        item.RealLongitudeDegrees,
                        ascendant);

            var realAngleUnwrapped =
                180.0
                - (
                    item.RealLongitudeDegrees
                    - ascendant
                );

            var realAnchor =
                NatalWheelCoordinates
                    .PointOnCircle(
                        wheel.Metrics.Center,
                        effectiveBaseRadius,
                        realAngle);

            NatalVisualPlacement?
                accepted = null;

            double acceptedVisualUnwrapped =
                double.NaN;

            var maximumAngularSteps =
                Math.Max(
                    24,
                    ordered.Length * 4);

            for (
                var angularStep = 0;
                angularStep <= maximumAngularSteps;
                angularStep++)
            {
                var visualUnwrapped =
                    realAngleUnwrapped
                    - angularStep
                    * angularStepDegrees;

                if (previousVisualUnwrapped is double previous
                    && visualUnwrapped
                        > previous + GeometryTolerance)
                {
                    continue;
                }

                var visualAngle =
                    NatalWheelCoordinates
                        .NormalizeDegrees(
                            visualUnwrapped);

                foreach (
                    var radialLevel
                    in EnumerateRadialLevels(
                        policy.MaximumRadialLevel))
                {
                    var visualRadius =
                        effectiveBaseRadius
                        + radialLevel
                        * policy.RadialStep;

                    if (!IsRadiusInsideSafeAnnulus(
                        visualRadius,
                        minimumSafeRadius,
                        maximumSafeRadius))
                    {
                        continue;
                    }

                    var visualCenter =
                        NatalWheelCoordinates
                            .PointOnCircle(
                                wheel.Metrics.Center,
                                visualRadius,
                                visualAngle);

                    var bounds =
                        CreateBounds(
                            visualCenter,
                            policy.GlyphSize,
                            policy.MinimumGap);

                    if (placed.Any(
                        existing =>
                            existing.Bounds
                                .Intersects(bounds)))
                    {
                        continue;
                    }

                    var displacement =
                        Distance(
                            realAnchor,
                            visualCenter);

                    var hasLeaderLine =
                        displacement
                        >= policy.LeaderLineThreshold;

                    accepted =
                        new NatalVisualPlacement(
                            item.Id,
                            item.RealLongitudeDegrees,
                            realAngle,
                            visualAngle,
                            radialLevel,
                            visualRadius,
                            realAnchor,
                            visualCenter,
                            bounds,
                            hasLeaderLine,
                            hasLeaderLine
                                ? realAnchor
                                : null,
                            hasLeaderLine
                                ? visualCenter
                                : null);

                    acceptedVisualUnwrapped =
                        visualUnwrapped;

                    break;
                }

                if (accepted is not null)
                {
                    break;
                }
            }

            if (accepted is null)
            {
                throw new InvalidOperationException(
                    $"Unable to place '{item.Id}' inside the safe annulus without overlap.");
            }

            placed.Add(
                accepted);

            previousVisualUnwrapped =
                acceptedVisualUnwrapped;
        }

        return new NatalObjectPlacementSnapshot(
            placed);
    }

    private static bool IsRadiusInsideSafeAnnulus(
        double radius,
        double minimumRadius,
        double maximumRadius)
        =>
            radius
                >= minimumRadius
                    - GeometryTolerance
            && radius
                <= maximumRadius
                    + GeometryTolerance;

    private static double CalculateAngularStepDegrees(
        double protectedSize,
        double minimumSafeRadius)
    {
        var ratio =
            protectedSize
            / (2.0 * minimumSafeRadius);

        ratio =
            Math.Clamp(
                ratio,
                0.0,
                1.0);

        var radians =
            2.0
            * Math.Asin(
                ratio);

        var degrees =
            radians
            * 180.0
            / Math.PI;

        return degrees
            * 1.05;
    }

    private static IEnumerable<int>
        EnumerateRadialLevels(
            int maximumLevel)
    {
        yield return 0;

        for (
            var level = 1;
            level <= maximumLevel;
            level++)
        {
            yield return level;
            yield return -level;
        }
    }

    private static ChartRect CreateBounds(
        ChartPoint center,
        double glyphSize,
        double minimumGap)
    {
        var protectedSize =
            glyphSize
            + minimumGap;

        return new ChartRect(
            center.X
                - protectedSize / 2.0,
            center.Y
                - protectedSize / 2.0,
            protectedSize,
            protectedSize);
    }

    private static double Distance(
        ChartPoint first,
        ChartPoint second)
    {
        var dx =
            second.X - first.X;

        var dy =
            second.Y - first.Y;

        return Math.Sqrt(
            dx * dx
            + dy * dy);
    }

    private static void EnsureUniqueIds(
        IReadOnlyList<NatalObjectLayoutInput>
            objects)
    {
        var duplicate =
            objects
                .GroupBy(
                    item => item.Id,
                    StringComparer.Ordinal)
                .FirstOrDefault(
                    group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new ArgumentException(
                $"Duplicate object Id '{duplicate.Key}'.");
        }
    }

    private static void ValidatePolicy(
        NatalGlyphLayoutPolicy policy)
    {
        if (!double.IsFinite(policy.GlyphSize)
            || !double.IsFinite(policy.MinimumGap)
            || !double.IsFinite(policy.RadialStep)
            || !double.IsFinite(policy.LeaderLineThreshold)
            || policy.GlyphSize <= 0.0
            || policy.MinimumGap < 0.0
            || policy.RadialStep <= 0.0
            || policy.LeaderLineThreshold < 0.0
            || policy.MaximumRadialLevel < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(policy));
        }
    }
}
