using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Styles;
using Miastro.Graphics.Scene.Natal.Aspects;

namespace Miastro.Graphics.Scene.Natal;

public sealed class NatalWheelSceneBuilder
{
    public NatalScene Build(
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot placements,
        IReadOnlyList<NatalSceneObjectInput> objects)
        =>
            Build(
                wheel,
                placements,
                objects,
                Array.Empty<NatalAspectSceneInput>(),
                new NatalAspectSceneOptions());

    public NatalScene Build(
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot placements,
        IReadOnlyList<NatalSceneObjectInput> objects,
        IReadOnlyList<NatalAspectSceneInput> aspects,
        NatalAspectSceneOptions? aspectOptions = null)
    {
        ArgumentNullException.ThrowIfNull(wheel);
        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(objects);

        var objectMap =
            objects.ToDictionary(
                x => x.Id,
                StringComparer.Ordinal);

        ValidateObjectMap(
            placements,
            objectMap);

        var nodes =
            new List<SceneNode>();

        AddBackground(
            nodes,
            wheel);

        AddZodiacRing(
            nodes,
            wheel);

        AddDegreeRing(
            nodes,
            wheel);

        AddHouses(
            nodes,
            wheel);

        AddAngles(
            nodes,
            wheel);

        AddAspectRing(
            nodes,
            wheel);

        AddObjects(
            nodes,
            wheel,
            placements,
            objectMap);

        foreach (
            var aspectNode
            in new NatalAspectSceneBuilder()
                .Build(
                    wheel,
                    placements,
                    aspects,
                    aspectOptions))
        {
            nodes.Add(
                aspectNode);
        }

        AddSoulCore(
            nodes,
            wheel);

        return new NatalScene(
            wheel.Metrics.Width,
            wheel.Metrics.Height,
            nodes.ToArray());
    }

    private static void AddBackground(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        nodes.Add(
            new CircleNode(
                "background-disc",
                SceneLayer.Background,
                wheel.Metrics.Center,
                wheel.Metrics.OuterRadius)
            {
                StyleKey =
                    NatalSceneStyleKeys.Background
            });
    }

    private static void AddZodiacRing(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        nodes.Add(
            new CircleNode(
                "zodiac-outer-ring",
                SceneLayer.ZodiacRing,
                wheel.Metrics.Center,
                wheel.Metrics.OuterRadius)
            {
                StyleKey =
                    NatalSceneStyleKeys.ZodiacBoundary
            });

        var glyphRadius =
            (
                wheel.Metrics.OuterRadius
                + wheel.Metrics.ZodiacInnerRadius
            )
            / 2.0;

        var glyphSize =
            24.0
            * wheel.Metrics.Scale;

        foreach (
            var sector
            in wheel.ZodiacSectors)
        {
            nodes.Add(
                new ArcNode(
                    $"zodiac-sector-{sector.SignIndex:00}",
                    SceneLayer.ZodiacRing,
                    wheel.Metrics.Center,
                    wheel.Metrics.OuterRadius,
                    sector.StartScreenAngleDegrees,
                    sector.SweepAngleDegrees)
                {
                    StyleKey =
                        NatalSceneStyleKeys.ZodiacBoundary
                });

            var centerAngle =
                Miastro.Graphics.Geometry
                    .NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        sector.CenterLongitudeDegrees,
                        wheel.AscendantLongitudeDegrees);

            var center =
                Miastro.Graphics.Geometry
                    .NatalWheelCoordinates
                    .PointOnCircle(
                        wheel.Metrics.Center,
                        glyphRadius,
                        centerAngle);

            nodes.Add(
                new GlyphNode(
                    $"zodiac-glyph-{sector.SignIndex:00}",
                    SceneLayer.ZodiacRing,
                    $"zodiac-{sector.SignIndex:00}",
                    center,
                    glyphSize,
                    BoundsFromCenter(
                        center,
                        glyphSize))
                {
                    StyleKey =
                        NatalSceneStyleKeys.ZodiacGlyph
                });
        }
    }

    private static void AddDegreeRing(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        nodes.Add(
            new CircleNode(
                "zodiac-degree-ring",
                SceneLayer.DegreeRing,
                wheel.Metrics.Center,
                wheel.Metrics.DegreeRingRadius)
            {
                StyleKey =
                    NatalSceneStyleKeys.DegreeBoundary
            });

        foreach (
            var tick
            in wheel.DegreeTicks)
        {
            nodes.Add(
                new LineNode(
                    $"degree-{tick.ZodiacDegree:000}",
                    SceneLayer.DegreeRing,
                    tick.OuterPoint,
                    tick.InnerPoint)
                {
                    StyleKey =
                        tick.Kind switch
                        {
                            DegreeTickKind.TenDegree =>
                                NatalSceneStyleKeys.DegreeTen,

                            DegreeTickKind.FiveDegree =>
                                NatalSceneStyleKeys.DegreeFive,

                            _ =>
                                NatalSceneStyleKeys.DegreeMinor
                        }
                });
        }
    }

    private static void AddHouses(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        var labelSize =
            16.0
            * wheel.Metrics.Scale;

        foreach (
            var cusp
            in wheel.HouseCusps)
        {
            nodes.Add(
                new LineNode(
                    $"house-cusp-{cusp.HouseNumber:00}",
                    SceneLayer.HouseLayer,
                    cusp.OuterPoint,
                    cusp.InnerPoint)
                {
                    StyleKey =
                        NatalSceneStyleKeys.HouseCusp
                });

            nodes.Add(
                new TextNode(
                    $"house-number-{cusp.HouseNumber:00}",
                    SceneLayer.HouseLayer,
                    cusp.HouseNumber.ToString(
                        System.Globalization
                            .CultureInfo.InvariantCulture),
                    cusp.HouseNumberPosition,
                    labelSize,
                    BoundsFromCenter(
                        cusp.HouseNumberPosition,
                        labelSize))
                {
                    StyleKey =
                        NatalSceneStyleKeys.HouseNumber
                });
        }
    }

    private static void AddAspectRing(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        nodes.Add(
            new CircleNode(
                "aspect-anchor-ring",
                SceneLayer.AspectLayer,
                wheel.Metrics.Center,
                wheel.Metrics.AspectRadius)
            {
                StyleKey =
                    NatalSceneStyleKeys.AspectRing
            });
    }

    private static void AddSoulCore(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        nodes.Add(
            new CircleNode(
                "soul-core",
                SceneLayer.AspectLayer,
                wheel.Metrics.Center,
                wheel.Metrics.SoulRadius)
            {
                StyleKey =
                    NatalSceneStyleKeys.SoulCore
            });
    }

    private static void AddAngles(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel)
    {
        var labelRadius =
            wheel.Metrics.OuterRadius
            + 34.0 * wheel.Metrics.Scale;

        var labelSize =
            14.0
            * wheel.Metrics.Scale;

        foreach (
            var axis
            in wheel.AngleAxes)
        {
            var key =
                axis.Kind switch
                {
                    NatalAngleKind.Ascendant => "ASC",
                    NatalAngleKind.Descendant => "DSC",
                    NatalAngleKind.Midheaven => "MC",
                    NatalAngleKind.ImumCoeli => "IC",
                    _ => throw new InvalidOperationException()
                };

            nodes.Add(
                new LineNode(
                    $"angle-axis-{key}",
                    SceneLayer.AngleLayer,
                    axis.OuterPoint,
                    axis.InnerPoint)
                {
                    StyleKey =
                        axis.Kind is NatalAngleKind.Ascendant
                            or NatalAngleKind.Midheaven
                                ? NatalSceneStyleKeys.AngleMajor
                                : NatalSceneStyleKeys.AngleMinor
                });

            ChartPoint labelCenter;

            if (axis.Kind
                == NatalAngleKind.Ascendant)
            {
                // ASC: encima del tramo exterior izquierdo y
                // desplazado ligeramente hacia dentro para evitar
                // cualquier recorte por el borde del canvas.
                labelCenter =
                    new ChartPoint(
                        axis.OuterPoint.X
                            + 10.0
                            * wheel.Metrics.Scale,
                        axis.OuterPoint.Y
                            - 13.0
                            * wheel.Metrics.Scale);
            }
            else if (axis.Kind
                == NatalAngleKind.Midheaven)
            {
                // MC: al lado derecho del tramo exterior.
                labelCenter =
                    new ChartPoint(
                        axis.OuterPoint.X
                            + 14.0
                            * wheel.Metrics.Scale,
                        axis.OuterPoint.Y);
            }
            else if (axis.Kind
                == NatalAngleKind.Descendant)
            {
                // DSC: encima del tramo exterior derecho y
                // desplazado hacia dentro para evitar recorte.
                labelCenter =
                    new ChartPoint(
                        axis.OuterPoint.X
                            - 10.0
                            * wheel.Metrics.Scale,
                        axis.OuterPoint.Y
                            - 13.0
                            * wheel.Metrics.Scale);
            }
            else
            {
                // IC: al lado derecho del tramo exterior inferior,
                // ligeramente elevado para mantenerlo dentro del canvas.
                labelCenter =
                    new ChartPoint(
                        axis.OuterPoint.X
                            + 14.0
                            * wheel.Metrics.Scale,
                        axis.OuterPoint.Y
                            - 10.0
                            * wheel.Metrics.Scale);
            }

            nodes.Add(
                new TextNode(
                    $"angle-label-{key}",
                    SceneLayer.AngleLayer,
                    key,
                    labelCenter,
                    labelSize,
                    BoundsFromCenter(
                        labelCenter,
                        labelSize * 1.8))
                {
                    StyleKey =
                        axis.Kind is NatalAngleKind.Ascendant
                            or NatalAngleKind.Midheaven
                                ? NatalSceneStyleKeys.AngleLabelMajor
                                : NatalSceneStyleKeys.AngleLabelMinor
                });
        }
    }

    private static void AddObjects(
        ICollection<SceneNode> nodes,
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot placements,
        IReadOnlyDictionary<
            string,
            NatalSceneObjectInput> objectMap)
    {
        foreach (
            var placement
            in placements.Placements)
        {
            var definition =
                objectMap[placement.Id];

            // No se dibuja una marca explícita de posición real.
            // La posición astrológica sigue preservada en placement
            // y se expone mediante interacción/tooltip.
            if (placement.HasLeaderLine
                && placement.LeaderLineStart is not null
                && placement.LeaderLineEnd is not null)
            {
                nodes.Add(
                    new LineNode(
                        $"leader-{placement.Id}",
                        definition.Layer,
                        placement.LeaderLineStart.Value,
                        placement.LeaderLineEnd.Value)
                    {
                        StyleKey =
                            NatalSceneStyleKeys.LeaderLine
                    });
            }

            nodes.Add(
                new GlyphNode(
                    $"object-glyph-{placement.Id}",
                    definition.Layer,
                    definition.GlyphKey,
                    placement.VisualCenter,
                    placement.GlyphSize,
                    placement.GlyphBounds)
                {
                    StyleKey =
                        definition.Layer
                            == SceneLayer.BodyLayer
                            ? NatalSceneStyleKeys.BodyGlyph
                            : NatalSceneStyleKeys.PointGlyph
                });
        }
    }

    private static bool IsInsideCanvas(
        ChartRect bounds,
        NatalWheelLayoutSnapshot wheel)
        =>
            bounds.Left >= 0.0
            && bounds.Top >= 0.0
            && bounds.Right
                <= wheel.Metrics.Width
            && bounds.Bottom
                <= wheel.Metrics.Height;

    private static ChartRect BoundsFromCenter(
        ChartPoint center,
        double width,
        double height)
        =>
            new(
                center.X - width / 2.0,
                center.Y - height / 2.0,
                width,
                height);

    private static readonly string[]
        ZodiacAbbreviations =
    [
        "Ari",
        "Tau",
        "Gem",
        "Cán",
        "Leo",
        "Vir",
        "Lib",
        "Esc",
        "Sag",
        "Cap",
        "Acu",
        "Pis"
    ];

    private static ChartRect BoundsFromCenter(
        ChartPoint center,
        double size)
        =>
            new(
                center.X - size / 2.0,
                center.Y - size / 2.0,
                size,
                size);

    private static void ValidateObjectMap(
        NatalObjectPlacementSnapshot placements,
        IReadOnlyDictionary<
            string,
            NatalSceneObjectInput> objectMap)
    {
        if (objectMap.Count
            != placements.Placements.Count)
        {
            throw new ArgumentException(
                "Scene object definitions must match placements.");
        }

        foreach (
            var placement
            in placements.Placements)
        {
            if (!objectMap.ContainsKey(
                placement.Id))
            {
                throw new ArgumentException(
                    $"Missing scene definition for '{placement.Id}'.");
            }
        }
    }
}
