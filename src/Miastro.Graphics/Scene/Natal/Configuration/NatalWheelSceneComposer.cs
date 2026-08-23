using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene.Natal.Aspects;

namespace Miastro.Graphics.Scene.Natal.Configuration;

/// <summary>
/// Applies visual configuration without recalculating astrology
/// or re-running object placement.
/// </summary>
public sealed class NatalWheelSceneComposer
{
    public NatalWheelSceneCompositionResult Compose(
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot placements,
        IReadOnlyList<NatalSceneObjectInput> objects,
        IReadOnlyList<NatalAspectSceneInput> aspects,
        NatalWheelSceneConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(wheel);
        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(objects);
        ArgumentNullException.ThrowIfNull(aspects);
        ArgumentNullException.ThrowIfNull(configuration);

        var detail =
            NatalWheelResponsivePolicy.Resolve(
                wheel.Metrics.Width,
                wheel.Metrics.Height);

        var visibleObjects =
            objects
                .Where(
                    item =>
                        item.Layer switch
                        {
                            SceneLayer.BodyLayer =>
                                configuration.Visibility
                                    .ShowPlanets,

                            SceneLayer.PointLayer =>
                                configuration.Visibility
                                    .ShowPoints,

                            _ => false
                        })
                .ToArray();

        var visibleIds =
            visibleObjects
                .Select(x => x.Id)
                .ToHashSet(
                    StringComparer.Ordinal);

        var visiblePlacements =
            new NatalObjectPlacementSnapshot(
                placements.Placements
                    .Where(
                        x => visibleIds.Contains(
                            x.Id))
                    .ToArray());

        var visibleAspects =
            configuration.Visibility.ShowAspects
                ? aspects
                    .Where(
                        x =>
                            visibleIds.Contains(
                                x.FirstObjectId)
                            && visibleIds.Contains(
                                x.SecondObjectId))
                    .ToArray()
                : Array.Empty<NatalAspectSceneInput>();

        var scene =
            new NatalWheelSceneBuilder()
                .Build(
                    wheel,
                    visiblePlacements,
                    visibleObjects,
                    visibleAspects,
                    new NatalAspectSceneOptions(
                        configuration.Visibility
                            .ShowAspects));

        var filteredNodes =
            scene.Nodes
                .Where(
                    node =>
                        KeepNode(
                            node,
                            configuration,
                            detail))
                .ToArray();

        return new NatalWheelSceneCompositionResult(
            new NatalScene(
                scene.Width,
                scene.Height,
                filteredNodes),
            configuration.Mode,
            detail);
    }

    private static bool KeepNode(
        SceneNode node,
        NatalWheelSceneConfiguration configuration,
        NatalWheelDetailLevel detail)
    {
        if (!configuration.Visibility.ShowCusps
            && node.Id.StartsWith(
                "house-cusp-",
                StringComparison.Ordinal))
        {
            return false;
        }

        if (!configuration.Visibility.ShowLabels
            && node is TextNode)
        {
            return false;
        }

        if (detail == NatalWheelDetailLevel.Compact
            && IsMinorDegreeTick(
                node.Id))
        {
            return false;
        }

        if (detail == NatalWheelDetailLevel.Minimal)
        {
            if (IsNonTenDegreeTick(
                node.Id))
            {
                return false;
            }

            if (node.Id.StartsWith(
                "house-number-",
                StringComparison.Ordinal))
            {
                return false;
            }

            if (node.Id is
                "angle-label-DSC"
                or "angle-label-IC")
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsMinorDegreeTick(
        string id)
    {
        if (!TryDegree(
            id,
            out var degree))
        {
            return false;
        }

        return degree % 5 != 0;
    }

    private static bool IsNonTenDegreeTick(
        string id)
    {
        if (!TryDegree(
            id,
            out var degree))
        {
            return false;
        }

        return degree % 10 != 0;
    }

    private static bool TryDegree(
        string id,
        out int degree)
    {
        degree = 0;

        if (!id.StartsWith(
            "degree-",
            StringComparison.Ordinal))
        {
            return false;
        }

        return int.TryParse(
            id.AsSpan(
                "degree-".Length),
            System.Globalization.NumberStyles.None,
            System.Globalization.CultureInfo.InvariantCulture,
            out degree);
    }
}
