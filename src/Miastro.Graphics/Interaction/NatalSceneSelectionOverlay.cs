using Miastro.Graphics.Scene;
using Miastro.Graphics.Styles;

namespace Miastro.Graphics.Interaction;

/// <summary>
/// Añade estado visual de selección a una escena natal ya construida.
///
/// No modifica layout, longitudes, aspectos ni geometría base.
/// Toda la interacción se representa mediante copias visuales
/// situadas en InteractionOverlay.
/// </summary>
public sealed class NatalSceneSelectionOverlay
{
    private const string SelectionPrefix =
        "selection-";

    private const string ObjectGlyphPrefix =
        "object-glyph-";

    public NatalScene Apply(
        NatalScene scene,
        IReadOnlyCollection<string>? selectedObjectIds = null,
        string? aspectFirstObjectId = null,
        string? aspectSecondObjectId = null)
    {
        ArgumentNullException.ThrowIfNull(
            scene);

        var baseNodes =
            scene.Nodes
                .Where(
                    node =>
                        !IsSelectionOverlayNode(
                            node))
                .ToArray();

        var selectedIds =
            NormalizeSelectedIds(
                selectedObjectIds);

        var overlayNodes =
            new List<SceneNode>();

        if (TryNormalizeAspectPair(
                aspectFirstObjectId,
                aspectSecondObjectId,
                out var first,
                out var second))
        {
            AddDimmingOverlays(
                baseNodes,
                selectedIds,
                first,
                second,
                overlayNodes);

            AddAspectHighlights(
                baseNodes,
                first,
                second,
                overlayNodes);
        }

        AddObjectHighlights(
            baseNodes,
            selectedIds,
            overlayNodes);

        return new NatalScene(
            scene.Width,
            scene.Height,
            baseNodes
                .Concat(
                    overlayNodes)
                .ToArray());
    }

    private static IReadOnlySet<string>
        NormalizeSelectedIds(
            IReadOnlyCollection<string>?
                selectedObjectIds)
        =>
            selectedObjectIds is null
                ? new HashSet<string>(
                    StringComparer.Ordinal)
                : selectedObjectIds
                    .Where(
                        id =>
                            !string.IsNullOrWhiteSpace(
                                id))
                    .Select(
                        id =>
                            id.Trim())
                    .ToHashSet(
                        StringComparer.Ordinal);

    private static bool TryNormalizeAspectPair(
        string? firstObjectId,
        string? secondObjectId,
        out string first,
        out string second)
    {
        first =
            firstObjectId?.Trim()
            ?? string.Empty;

        second =
            secondObjectId?.Trim()
            ?? string.Empty;

        return first.Length > 0
            && second.Length > 0
            && !string.Equals(
                first,
                second,
                StringComparison.Ordinal);
    }

    private static void AddDimmingOverlays(
        IReadOnlyList<SceneNode> baseNodes,
        IReadOnlySet<string> selectedObjectIds,
        string firstObjectId,
        string secondObjectId,
        ICollection<SceneNode> overlayNodes)
    {
        foreach (
            var source
            in baseNodes
                .OfType<GlyphNode>()
                .Where(
                    node =>
                        node.Id.StartsWith(
                            ObjectGlyphPrefix,
                            StringComparison.Ordinal))
                .OrderBy(
                    node =>
                        node.Id,
                    StringComparer.Ordinal))
        {
            var objectId =
                source.Id[
                    ObjectGlyphPrefix.Length..];

            if (selectedObjectIds.Contains(
                    objectId))
            {
                continue;
            }

            overlayNodes.Add(
                new GlyphNode(
                    $"{SelectionPrefix}dim-{source.Id}",
                    SceneLayer.InteractionOverlay,
                    source.GlyphKey,
                    source.Position,
                    source.Size,
                    source.Bounds)
                {
                    StyleKey =
                        NatalSceneStyleKeys
                            .InteractionDimmed
                });
        }

        foreach (
            var source
            in baseNodes
                .OfType<LineNode>()
                .Where(
                    node =>
                        node.Layer
                            == SceneLayer.AspectLayer)
                .Where(
                    node =>
                        !MatchesAspectPair(
                            node.Id,
                            firstObjectId,
                            secondObjectId))
                .OrderBy(
                    node =>
                        node.Id,
                    StringComparer.Ordinal))
        {
            overlayNodes.Add(
                new LineNode(
                    $"{SelectionPrefix}dim-{source.Id}",
                    SceneLayer.InteractionOverlay,
                    source.Start,
                    source.End)
                {
                    StyleKey =
                        NatalSceneStyleKeys
                            .InteractionDimmed
                });
        }
    }

    private static void AddObjectHighlights(
        IReadOnlyList<SceneNode> baseNodes,
        IReadOnlySet<string> selectedObjectIds,
        ICollection<SceneNode> overlayNodes)
    {
        foreach (
            var objectId
            in selectedObjectIds
                .OrderBy(
                    id =>
                        id,
                    StringComparer.Ordinal))
        {
            var nodeId =
                $"{ObjectGlyphPrefix}{objectId}";

            var source =
                baseNodes
                    .OfType<GlyphNode>()
                    .FirstOrDefault(
                        node =>
                            string.Equals(
                                node.Id,
                                nodeId,
                                StringComparison.Ordinal));

            if (source is null)
            {
                continue;
            }

            overlayNodes.Add(
                new GlyphNode(
                    $"{SelectionPrefix}object-{objectId}",
                    SceneLayer.InteractionOverlay,
                    source.GlyphKey,
                    source.Position,
                    source.Size,
                    source.Bounds)
                {
                    StyleKey =
                        NatalSceneStyleKeys
                            .InteractionSelected
                });
        }
    }

    private static void AddAspectHighlights(
        IReadOnlyList<SceneNode> baseNodes,
        string firstObjectId,
        string secondObjectId,
        ICollection<SceneNode> overlayNodes)
    {
        var matchingLines =
            baseNodes
                .OfType<LineNode>()
                .Where(
                    node =>
                        node.Layer
                            == SceneLayer.AspectLayer
                        && MatchesAspectPair(
                            node.Id,
                            firstObjectId,
                            secondObjectId))
                .OrderBy(
                    node =>
                        node.Id,
                    StringComparer.Ordinal)
                .ToArray();

        foreach (var source in matchingLines)
        {
            overlayNodes.Add(
                new LineNode(
                    $"{SelectionPrefix}{source.Id}",
                    SceneLayer.InteractionOverlay,
                    source.Start,
                    source.End)
                {
                    StyleKey =
                        NatalSceneStyleKeys
                            .InteractionSelected
                });
        }
    }

    private static bool MatchesAspectPair(
        string nodeId,
        string firstObjectId,
        string secondObjectId)
    {
        var forwardPrefix =
            $"aspect-{firstObjectId}-{secondObjectId}-";

        var reversePrefix =
            $"aspect-{secondObjectId}-{firstObjectId}-";

        return nodeId.StartsWith(
                forwardPrefix,
                StringComparison.Ordinal)
            || nodeId.StartsWith(
                reversePrefix,
                StringComparison.Ordinal);
    }

    private static bool IsSelectionOverlayNode(
        SceneNode node)
        =>
            node.Layer
                == SceneLayer.InteractionOverlay
            && node.Id.StartsWith(
                SelectionPrefix,
                StringComparison.Ordinal);
}
