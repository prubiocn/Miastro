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

            result.Add(
                new LineNode(
                    $"aspect-{aspect.Id}",
                    SceneLayer.AspectLayer,
                    start,
                    end)
                {
                    StyleKey =
                        aspect.VisualClass
                            == NatalAspectVisualClass.Major
                                ? NatalSceneStyleKeys.AspectMajor
                                : NatalSceneStyleKeys.AspectSecondary
                });
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
