using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;

namespace Miastro.Graphics.Interaction;

/// <summary>
/// Hit testing over final visual scene geometry.
///
/// Uses the actual displaced GlyphNode bounds.
/// It does not infer hits from astrological longitude.
/// </summary>
public sealed class NatalSceneHitTester
{
    private const string ObjectGlyphPrefix =
        "object-glyph-";

    public NatalHitTestResult? HitTestViewport(
        NatalScene scene,
        double x,
        double y,
        double viewportWidth,
        double viewportHeight,
        double tolerance = 0.0)
    {
        ArgumentNullException.ThrowIfNull(scene);

        if (!double.IsFinite(x)
            || !double.IsFinite(y)
            || !double.IsFinite(viewportWidth)
            || !double.IsFinite(viewportHeight)
            || viewportWidth <= 0.0
            || viewportHeight <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewportWidth));
        }

        if (!double.IsFinite(tolerance)
            || tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tolerance));
        }

        var scale =
            Math.Min(
                viewportWidth / scene.Width,
                viewportHeight / scene.Height);

        var renderedWidth =
            scene.Width * scale;

        var renderedHeight =
            scene.Height * scale;

        var offsetX =
            (
                viewportWidth
                - renderedWidth
            )
            / 2.0;

        var offsetY =
            (
                viewportHeight
                - renderedHeight
            )
            / 2.0;

        if (x < offsetX
            || x > offsetX + renderedWidth
            || y < offsetY
            || y > offsetY + renderedHeight)
        {
            return null;
        }

        var scenePoint =
            new ChartPoint(
                (x - offsetX) / scale,
                (y - offsetY) / scale);

        var sceneTolerance =
            tolerance / scale;

        return HitTest(
            scene,
            scenePoint,
            sceneTolerance);
    }

    public IReadOnlyList<string>
        GetSelectableObjectIds(
            NatalScene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        return scene.OrderedNodes
            .OfType<GlyphNode>()
            .Where(
                IsSelectableObjectGlyph)
            .Select(
                glyph =>
                    glyph.Id[
                        ObjectGlyphPrefix.Length..])
            .Distinct(
                StringComparer.Ordinal)
            .ToArray();
    }

    public NatalHitTestResult? HitTest(
        NatalScene scene,
        ChartPoint point,
        double tolerance = 0.0)
    {
        ArgumentNullException.ThrowIfNull(scene);

        if (!double.IsFinite(point.X)
            || !double.IsFinite(point.Y))
        {
            throw new ArgumentOutOfRangeException(
                nameof(point));
        }

        if (!double.IsFinite(tolerance)
            || tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tolerance));
        }

        return scene.OrderedNodes
            .OfType<GlyphNode>()
            .Where(IsSelectableObjectGlyph)
            .Where(
                glyph =>
                    Expand(
                            glyph.Bounds,
                            tolerance)
                        .Contains(point))
            .OrderByDescending(
                glyph => (int)glyph.Layer)
            .ThenBy(
                glyph => glyph.Id,
                StringComparer.Ordinal)
            .Select(ToResult)
            .FirstOrDefault();
    }

    private static bool IsSelectableObjectGlyph(
        GlyphNode glyph)
        =>
            glyph.Id.StartsWith(
                ObjectGlyphPrefix,
                StringComparison.Ordinal)
            && glyph.Layer
                is SceneLayer.BodyLayer
                or SceneLayer.PointLayer;

    private static NatalHitTestResult ToResult(
        GlyphNode glyph)
    {
        var objectId =
            glyph.Id[
                ObjectGlyphPrefix.Length..];

        var kind =
            glyph.Layer
                == SceneLayer.BodyLayer
                    ? NatalHitTargetKind.Body
                    : NatalHitTargetKind.Point;

        return new NatalHitTestResult(
            objectId,
            kind,
            glyph.Bounds);
    }

    private static ChartRect Expand(
        ChartRect bounds,
        double tolerance)
        =>
            tolerance == 0.0
                ? bounds
                : new ChartRect(
                    bounds.Left - tolerance,
                    bounds.Top - tolerance,
                    bounds.Width + tolerance * 2.0,
                    bounds.Height + tolerance * 2.0);
}
