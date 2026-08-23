using Miastro.Graphics.Glyphs;
using Miastro.Graphics.Scene;
using SkiaSharp;

namespace Miastro.Graphics.Skia.Rendering;

public sealed class SkiaNatalSceneRenderer
{
    public byte[] RenderPng(
        NatalScene scene,
        int pixelWidth,
        int pixelHeight)
    {
        ArgumentNullException.ThrowIfNull(scene);

        if (pixelWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pixelWidth));
        }

        if (pixelHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pixelHeight));
        }

        using var surface =
            SKSurface.Create(
                new SKImageInfo(
                    pixelWidth,
                    pixelHeight,
                    SKColorType.Rgba8888,
                    SKAlphaType.Premul));

        if (surface is null)
        {
            throw new InvalidOperationException(
                "Unable to create Skia surface.");
        }

        var canvas =
            surface.Canvas;

        canvas.Clear(
            new SKColor(
                250,
                248,
                243,
                255));

        canvas.Save();

        canvas.ClipRect(
            new SKRect(
                0,
                0,
                pixelWidth,
                pixelHeight));

        var sx =
            pixelWidth
            / (float)scene.Width;

        var sy =
            pixelHeight
            / (float)scene.Height;

        canvas.Scale(
            sx,
            sy);

        foreach (
            var node
            in scene.OrderedNodes)
        {
            DrawNode(
                canvas,
                node);
        }

        canvas.Restore();
        canvas.Flush();

        using var image =
            surface.Snapshot();

        using var data =
            image.Encode(
                SKEncodedImageFormat.Png,
                100);

        return data.ToArray();
    }

    private static void DrawNode(
        SKCanvas canvas,
        SceneNode node)
    {
        switch (node)
        {
            case CircleNode circle:
                DrawCircle(
                    canvas,
                    circle);
                break;

            case ArcNode arc:
                DrawArc(
                    canvas,
                    arc);
                break;

            case LineNode line:
                DrawLine(
                    canvas,
                    line);
                break;

            case GlyphNode glyph:
                DrawGlyph(
                    canvas,
                    glyph);
                break;

            case TextNode text:
                DrawTextPlaceholder(
                    canvas,
                    text);
                break;

            case PathNode path:
                DrawPath(
                    canvas,
                    path);
                break;

            case GroupNode group:
                foreach (
                    var child
                    in group.Children
                        .OrderBy(
                            x => (int)x.Layer)
                        .ThenBy(
                            x => x.Id,
                            StringComparer.Ordinal))
                {
                    DrawNode(
                        canvas,
                        child);
                }

                break;
        }
    }

    private static void DrawCircle(
        SKCanvas canvas,
        CircleNode node)
    {
        using var paint =
            CreateStrokePaint(
                node.Layer);

        canvas.DrawCircle(
            (float)node.Center.X,
            (float)node.Center.Y,
            (float)node.Radius,
            paint);
    }

    private static void DrawArc(
        SKCanvas canvas,
        ArcNode node)
    {
        using var paint =
            CreateStrokePaint(
                node.Layer);

        var radius =
            (float)node.Radius;

        var rect =
            new SKRect(
                (float)node.Center.X - radius,
                (float)node.Center.Y - radius,
                (float)node.Center.X + radius,
                (float)node.Center.Y + radius);

        canvas.DrawArc(
            rect,
            (float)node.StartAngleDegrees,
            (float)node.SweepAngleDegrees,
            false,
            paint);
    }

    private static void DrawLine(
        SKCanvas canvas,
        LineNode node)
    {
        using var paint =
            CreateStrokePaint(
                node.Layer);

        canvas.DrawLine(
            (float)node.Start.X,
            (float)node.Start.Y,
            (float)node.End.X,
            (float)node.End.Y,
            paint);
    }

    private static void DrawGlyph(
        SKCanvas canvas,
        GlyphNode node)
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        using var paint =
            CreateStrokePaint(
                node.Layer);

        if (!catalog.TryGet(
            node.GlyphKey,
            out var glyph))
        {
            DrawUnknownGlyph(
                canvas,
                node,
                paint);

            return;
        }

        var size =
            (float)Math.Max(
                4.0,
                node.Size);

        var centerX =
            (float)node.Position.X;

        var centerY =
            (float)node.Position.Y;

        foreach (
            var circle
            in glyph.Circles)
        {
            canvas.DrawCircle(
                centerX
                    + (float)circle.Center.X
                    * size,
                centerY
                    + (float)circle.Center.Y
                    * size,
                (float)circle.Radius
                    * size,
                paint);
        }

        foreach (
            var stroke
            in glyph.Strokes)
        {
            if (stroke.Points.Count < 2)
            {
                continue;
            }

            using var builder =
                new SKPathBuilder();

            builder.MoveTo(
                centerX
                    + (float)stroke.Points[0].X
                    * size,
                centerY
                    + (float)stroke.Points[0].Y
                    * size);

            for (
                var i = 1;
                i < stroke.Points.Count;
                i++)
            {
                builder.LineTo(
                    centerX
                        + (float)stroke.Points[i].X
                        * size,
                    centerY
                        + (float)stroke.Points[i].Y
                        * size);
            }

            if (stroke.Closed)
            {
                builder.Close();
            }

            using var path =
                builder.Detach();

            canvas.DrawPath(
                path,
                paint);
        }
    }

    private static void DrawUnknownGlyph(
        SKCanvas canvas,
        GlyphNode node,
        SKPaint paint)
    {
        var half =
            (float)Math.Max(
                2.0,
                node.Size * 0.30);

        var x =
            (float)node.Position.X;

        var y =
            (float)node.Position.Y;

        canvas.DrawLine(
            x,
            y - half,
            x + half,
            y,
            paint);

        canvas.DrawLine(
            x + half,
            y,
            x,
            y + half,
            paint);

        canvas.DrawLine(
            x,
            y + half,
            x - half,
            y,
            paint);

        canvas.DrawLine(
            x - half,
            y,
            x,
            y - half,
            paint);
    }

    private static void DrawTextPlaceholder(
        SKCanvas canvas,
        TextNode node)
    {
        using var paint =
            CreateStrokePaint(
                node.Layer);

        var rect =
            new SKRect(
                (float)node.Bounds.Left,
                (float)node.Bounds.Top,
                (float)node.Bounds.Right,
                (float)node.Bounds.Bottom);

        canvas.DrawRect(
            rect,
            paint);
    }

    private static void DrawPath(
        SKCanvas canvas,
        PathNode node)
    {
        if (node.Points.Count == 0)
        {
            return;
        }

        using var paint =
            CreateStrokePaint(
                node.Layer);

        using var builder =
            new SKPathBuilder();

        builder.MoveTo(
            (float)node.Points[0].X,
            (float)node.Points[0].Y);

        for (
            var i = 1;
            i < node.Points.Count;
            i++)
        {
            builder.LineTo(
                (float)node.Points[i].X,
                (float)node.Points[i].Y);
        }

        if (node.Closed)
        {
            builder.Close();
        }

        using var path =
            builder.Detach();

        canvas.DrawPath(
            path,
            paint);
    }

    private static SKPaint CreateStrokePaint(
        SceneLayer layer)
    {
        var paint =
            new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = StrokeWidth(layer),
                Color = LayerColor(layer),
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

        return paint;
    }

    private static float StrokeWidth(
        SceneLayer layer)
        =>
            layer switch
            {
                SceneLayer.AngleLayer => 2.2f,
                SceneLayer.BodyLayer => 2.0f,
                SceneLayer.PointLayer => 1.8f,
                SceneLayer.AspectLayer => 1.2f,
                SceneLayer.InteractionOverlay => 2.5f,
                _ => 1.0f
            };

    private static SKColor LayerColor(
        SceneLayer layer)
        =>
            layer switch
            {
                SceneLayer.Background =>
                    new SKColor(
                        218,
                        214,
                        205),

                SceneLayer.ZodiacRing =>
                    new SKColor(
                        94,
                        108,
                        120),

                SceneLayer.DegreeRing =>
                    new SKColor(
                        145,
                        143,
                        136),

                SceneLayer.HouseLayer =>
                    new SKColor(
                        120,
                        116,
                        108),

                SceneLayer.AngleLayer =>
                    new SKColor(
                        70,
                        86,
                        100),

                SceneLayer.BodyLayer =>
                    new SKColor(
                        50,
                        54,
                        57),

                SceneLayer.PointLayer =>
                    new SKColor(
                        112,
                        91,
                        61),

                SceneLayer.AspectLayer =>
                    new SKColor(
                        105,
                        115,
                        125),

                SceneLayer.LabelLayer =>
                    new SKColor(
                        50,
                        54,
                        57),

                _ =>
                    new SKColor(
                        75,
                        80,
                        84)
            };
}
