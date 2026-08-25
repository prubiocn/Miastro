using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Styles;

namespace Miastro.Graphics.Scene.Natal.Aspects;

/// <summary>
/// Converts already-known aspects into Scene Graph geometry.
///
/// No aspect rules, orbs or astronomical calculations live here.
/// </summary>
public sealed class NatalAspectSceneBuilder
{
    public IReadOnlyList<SceneNode> Build(
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot placements,
        IReadOnlyList<NatalAspectSceneInput> aspects,
        NatalAspectSceneOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(wheel);
        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(aspects);

        options ??=
            new NatalAspectSceneOptions();

        if (!options.ShowAspects)
        {
            return Array.Empty<SceneNode>();
        }

        EnsureUniqueAspectIds(
            aspects);

        var placementMap =
            placements.Placements
                .ToDictionary(
                    x => x.Id,
                    StringComparer.Ordinal);

        var result =
            new List<SceneNode>();

        foreach (
            var aspect
            in aspects
                .OrderBy(
                    x => x.Id,
                    StringComparer.Ordinal))
        {
            if (!placementMap.TryGetValue(
                    aspect.FirstObjectId,
                    out var first)
                || !placementMap.TryGetValue(
                    aspect.SecondObjectId,
                    out var second))
            {
                continue;
            }

            var start =
                NatalWheelCoordinates.PointOnCircle(
                    wheel.Metrics.Center,
                    wheel.Metrics.AspectRadius,
                    first.RealScreenAngleDegrees);

            var end =
                NatalWheelCoordinates.PointOnCircle(
                    wheel.Metrics.Center,
                    wheel.Metrics.AspectRadius,
                    second.RealScreenAngleDegrees);

            var styleKey =
                aspect.VisualClass
                    == NatalAspectVisualClass.Major
                        ? NatalSceneStyleKeys.AspectMajor
                        : NatalSceneStyleKeys.AspectSecondary;

            var visibleSegments =
                ClipOutsideCircle(
                    start,
                    end,
                    wheel.Metrics.Center,
                    wheel.Metrics.SoulRadius);

            if (visibleSegments.Count == 1)
            {
                result.Add(
                    new LineNode(
                        $"aspect-{aspect.Id}",
                        SceneLayer.AspectLayer,
                        visibleSegments[0].Start,
                        visibleSegments[0].End)
                    {
                        StyleKey =
                            styleKey
                    });
            }
            else
            {
                for (
                    var segmentIndex = 0;
                    segmentIndex < visibleSegments.Count;
                    segmentIndex++)
                {
                    var segment =
                        visibleSegments[
                            segmentIndex];

                    result.Add(
                        new LineNode(
                            $"aspect-{aspect.Id}-{segmentIndex + 1}",
                            SceneLayer.AspectLayer,
                            segment.Start,
                            segment.End)
                        {
                            StyleKey =
                                styleKey
                        });
                }
            }
        }

        return result;
    }

    private static IReadOnlyList<(
        ChartPoint Start,
        ChartPoint End)>
        ClipOutsideCircle(
            ChartPoint start,
            ChartPoint end,
            ChartPoint center,
            double radius)
    {
        const double tolerance =
            1e-9;

        var dx =
            end.X - start.X;

        var dy =
            end.Y - start.Y;

        var fx =
            start.X - center.X;

        var fy =
            start.Y - center.Y;

        var a =
            dx * dx
            + dy * dy;

        if (a <= tolerance)
        {
            return
            [
                (
                    start,
                    end
                )
            ];
        }

        var b =
            2.0
            * (
                fx * dx
                + fy * dy
            );

        var c =
            fx * fx
            + fy * fy
            - radius * radius;

        var discriminant =
            b * b
            - 4.0 * a * c;

        if (discriminant <= tolerance)
        {
            return
            [
                (
                    start,
                    end
                )
            ];
        }

        var root =
            Math.Sqrt(
                discriminant);

        var t1 =
            (
                -b - root
            )
            / (2.0 * a);

        var t2 =
            (
                -b + root
            )
            / (2.0 * a);

        if (t1 > t2)
        {
            (
                t1,
                t2
            ) =
                (
                    t2,
                    t1
                );
        }

        if (t2 <= tolerance
            || t1 >= 1.0 - tolerance)
        {
            return
            [
                (
                    start,
                    end
                )
            ];
        }

        t1 =
            Math.Clamp(
                t1,
                0.0,
                1.0);

        t2 =
            Math.Clamp(
                t2,
                0.0,
                1.0);

        if (t2 - t1 <= tolerance)
        {
            return
            [
                (
                    start,
                    end
                )
            ];
        }

        var firstIntersection =
            new ChartPoint(
                start.X + dx * t1,
                start.Y + dy * t1);

        var secondIntersection =
            new ChartPoint(
                start.X + dx * t2,
                start.Y + dy * t2);

        var result =
            new List<(
                ChartPoint Start,
                ChartPoint End)>();

        if (t1 > tolerance)
        {
            result.Add(
                (
                    start,
                    firstIntersection
                ));
        }

        if (t2 < 1.0 - tolerance)
        {
            result.Add(
                (
                    secondIntersection,
                    end
                ));
        }

        return result;
    }

    private static void EnsureUniqueAspectIds(
        IReadOnlyList<NatalAspectSceneInput> aspects)
    {
        var duplicate =
            aspects
                .GroupBy(
                    x => x.Id,
                    StringComparer.Ordinal)
                .FirstOrDefault(
                    x => x.Count() > 1);

        if (duplicate is not null)
        {
            throw new ArgumentException(
                $"Duplicate aspect Id '{duplicate.Key}'.");
        }
    }
}
