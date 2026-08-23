using Miastro.Graphics.Glyphs;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Styles;
using Miastro.Graphics.Skia.Typography;
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
                DrawText(
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
                node);

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
                node);

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
                node);

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
                node);

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

    private static void DrawText(
        SKCanvas canvas,
        TextNode node)
    {
        using var typography =
            new SkiaTypographyProvider();

        using var paint =
            CreateStrokePaint(
                node);

        using var font =
            new SKFont(
                typography.Typeface,
                (float)Math.Max(
                    1.0,
                    node.Size));

        paint.Style =
            SKPaintStyle.Fill;

        var textWidth =
            font.MeasureText(
                node.Text);

        var metrics =
            font.Metrics;

        var baseline =
            (float)node.Position.Y
            - (
                metrics.Ascent
                + metrics.Descent
              )
              / 2.0f;

        var x =
            (float)node.Position.X
            - textWidth / 2.0f;

        canvas.DrawText(
            node.Text,
            x,
            baseline,
            SKTextAlign.Left,
            font,
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
                node);

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
        SceneNode node)
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var style =
            ResolveStyle(
                catalog,
                node);

        var alpha =
            (byte)Math.Round(
                style.StrokeColor.Alpha
                * style.Opacity);

        var paint =
            new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth =
                    (float)Math.Max(
                        0.1,
                        style.StrokeWidth),
                Color =
                    new SKColor(
                        style.StrokeColor.Red,
                        style.StrokeColor.Green,
                        style.StrokeColor.Blue,
                        alpha),
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

        ApplyLinePattern(
            paint,
            style);

        return paint;
    }

    private static SceneStyle ResolveStyle(
        NatalSceneStyleCatalog catalog,
        SceneNode node)
    {
        if (!string.IsNullOrWhiteSpace(
            node.StyleKey)
            && catalog.TryGet(
                node.StyleKey,
                out var style))
        {
            return style;
        }

        var fallbackKey =
            node.Layer switch
            {
                SceneLayer.Background =>
                    NatalSceneStyleKeys.Background,

                SceneLayer.ZodiacRing =>
                    NatalSceneStyleKeys.ZodiacBoundary,

                SceneLayer.DegreeRing =>
                    NatalSceneStyleKeys.DegreeMinor,

                SceneLayer.HouseLayer =>
                    NatalSceneStyleKeys.HouseCusp,

                SceneLayer.AngleLayer =>
                    NatalSceneStyleKeys.AngleMajor,

                SceneLayer.BodyLayer =>
                    NatalSceneStyleKeys.BodyGlyph,

                SceneLayer.PointLayer =>
                    NatalSceneStyleKeys.PointGlyph,

                SceneLayer.AspectLayer =>
                    NatalSceneStyleKeys.AspectMajor,

                SceneLayer.LabelLayer =>
                    NatalSceneStyleKeys.LabelPrimary,

                _ =>
                    NatalSceneStyleKeys.InteractionSelected
            };

        return catalog.GetRequired(
            fallbackKey);
    }

    private static void ApplyLinePattern(
        SKPaint paint,
        SceneStyle style)
    {
        if (style.LinePattern
            == SceneLinePattern.Solid)
        {
            return;
        }

        var width =
            (float)Math.Max(
                1.0,
                style.StrokeWidth);

        var intervals =
            style.LinePattern
                == SceneLinePattern.Dashed
                    ? new[]
                    {
                        width * 5.0f,
                        width * 3.0f
                    }
                    : new[]
                    {
                        width,
                        width * 2.5f
                    };

        paint.PathEffect =
            SKPathEffect.CreateDash(
                intervals,
                0);
    }
}
