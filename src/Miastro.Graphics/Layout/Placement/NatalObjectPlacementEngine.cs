using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout.Placement;

public sealed class NatalObjectPlacementEngine
{
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

        foreach (var item in ordered)
        {
            var realAngle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        item.RealLongitudeDegrees,
                        ascendant);

            var realAnchor =
                NatalWheelCoordinates
                    .PointOnCircle(
                        wheel.Metrics.Center,
                        baseRadius,
                        realAngle);

            NatalVisualPlacement?
                accepted = null;

            foreach (
                var radialLevel
                in EnumerateRadialLevels(
                    policy.MaximumRadialLevel))
            {
                var visualRadius =
                    baseRadius
                    + radialLevel
                    * policy.RadialStep;

                if (visualRadius <= 0.0)
                {
                    continue;
                }

                var visualCenter =
                    NatalWheelCoordinates
                        .PointOnCircle(
                            wheel.Metrics.Center,
                            visualRadius,
                            realAngle);

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
                        realAngle,
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

                break;
            }

            if (accepted is null)
            {
                throw new InvalidOperationException(
                    $"Unable to place '{item.Id}' without overlap.");
            }

            placed.Add(
                accepted);
        }

        return new NatalObjectPlacementSnapshot(
            placed);
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
        if (policy.GlyphSize <= 0.0
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
