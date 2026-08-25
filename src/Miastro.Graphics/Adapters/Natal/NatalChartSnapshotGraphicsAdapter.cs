using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;

namespace Miastro.Graphics.Adapters.Natal;

/// <summary>
/// Adapts the persisted Phase 6 natal read model to Phase 7
/// graphics contracts.
///
/// It performs no astronomy, aspect detection or persistence access.
/// </summary>
public sealed class NatalChartSnapshotGraphicsAdapter
{
    private static readonly HashSet<AstrologicalObjectId>
        DefaultVisibleObjects =
        [
            AstrologicalObjectId.Sun,
            AstrologicalObjectId.Moon,
            AstrologicalObjectId.Mercury,
            AstrologicalObjectId.Venus,
            AstrologicalObjectId.Mars,
            AstrologicalObjectId.Jupiter,
            AstrologicalObjectId.Saturn,
            AstrologicalObjectId.Uranus,
            AstrologicalObjectId.Neptune,
            AstrologicalObjectId.Pluto,
            AstrologicalObjectId.Chiron,
            AstrologicalObjectId.Ascendant,
            AstrologicalObjectId.Midheaven
        ];

    public NatalSnapshotGraphicsModel Adapt(
        NatalChartSnapshotReadModel snapshot,
        double width,
        double height,
        NatalSnapshotGraphicsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        options ??=
            new NatalSnapshotGraphicsOptions();

        ValidateSnapshot(
            snapshot);

        var placementMap =
            snapshot.Placements
                .ToDictionary(
                    x => x.ObjectId);

        var ascendant =
            placementMap[
                AstrologicalObjectId.Ascendant]
                .LongitudeDegrees;

        var midheaven =
            placementMap[
                AstrologicalObjectId.Midheaven]
                .LongitudeDegrees;

        var cuspLongitudes =
            snapshot.HouseCusps
                .OrderBy(
                    x => x.HouseNumber)
                .Select(
                    x => x.LongitudeDegrees)
                .ToArray();

        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    width,
                    height,
                    ascendant,
                    midheaven,
                    cuspLongitudes);

        var selectedPlacements =
            snapshot.Placements
                .Where(
                    x =>
                        IsVisibleObject(
                            x.ObjectId,
                            options))
                .OrderBy(
                    x => (int)x.ObjectId)
                .ToArray();

        var placementInputs =
            selectedPlacements
                .Select(
                    x =>
                        new NatalObjectLayoutInput(
                            ObjectKey(
                                x.ObjectId),
                            x.LongitudeDegrees))
                .ToArray();

        var visualPlacements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    placementInputs);

        var objects =
            selectedPlacements
                .Select(
                    x =>
                        new NatalSceneObjectInput(
                            ObjectKey(
                                x.ObjectId),
                            GlyphKey(
                                x.ObjectId),
                            SceneLayerFor(
                                x.ObjectId),
                            isRetrograde:
                                x.Motion
                                    == MotionState.Retrograde))
                .ToArray();

        var visibleIds =
            selectedPlacements
                .Select(x => x.ObjectId)
                .ToHashSet();

        var aspects =
            snapshot.Aspects
                .Where(
                    x =>
                        visibleIds.Contains(
                            x.FirstObject)
                        && visibleIds.Contains(
                            x.SecondObject))
                .OrderBy(
                    x => (int)x.FirstObject)
                .ThenBy(
                    x => (int)x.SecondObject)
                .ThenBy(
                    x => (int)x.Kind)
                .Select(
                    x =>
                        new NatalAspectSceneInput(
                            AspectKey(x),
                            ObjectKey(
                                x.FirstObject),
                            ObjectKey(
                                x.SecondObject),
                            AspectClass(
                                x.Kind)))
                .ToArray();

        return new NatalSnapshotGraphicsModel(
            snapshot.Id,
            wheel,
            visualPlacements,
            objects,
            aspects);
    }

    private static void ValidateSnapshot(
        NatalChartSnapshotReadModel snapshot)
    {
        if (snapshot.HouseCusps.Count != 12)
        {
            throw new ArgumentException(
                "The natal graphics adapter requires exactly 12 house cusps.",
                nameof(snapshot));
        }

        var expectedHouses =
            Enumerable.Range(
                1,
                12)
                .ToArray();

        var actualHouses =
            snapshot.HouseCusps
                .Select(
                    x => x.HouseNumber)
                .OrderBy(
                    x => x)
                .ToArray();

        if (!expectedHouses.SequenceEqual(
            actualHouses))
        {
            throw new ArgumentException(
                "House cusps must contain houses 1 through 12 exactly once.",
                nameof(snapshot));
        }

        var duplicateObject =
            snapshot.Placements
                .GroupBy(
                    x => x.ObjectId)
                .FirstOrDefault(
                    x => x.Count() > 1);

        if (duplicateObject is not null)
        {
            throw new ArgumentException(
                $"Duplicate natal placement '{duplicateObject.Key}'.",
                nameof(snapshot));
        }

        var objects =
            snapshot.Placements
                .Select(
                    x => x.ObjectId)
                .ToHashSet();

        if (!objects.Contains(
                AstrologicalObjectId.Ascendant)
            || !objects.Contains(
                AstrologicalObjectId.Midheaven))
        {
            throw new ArgumentException(
                "The natal snapshot must contain Ascendant and Midheaven placements.",
                nameof(snapshot));
        }
    }

    private static bool IsVisibleObject(
        AstrologicalObjectId objectId,
        NatalSnapshotGraphicsOptions options)
        =>
            DefaultVisibleObjects.Contains(
                objectId)
            || (
                options.IncludeOptionalPoints
                && objectId
                    is AstrologicalObjectId.NorthTrueNode
                    or AstrologicalObjectId.SouthNode
                    or AstrologicalObjectId.MeanLilith
                    or AstrologicalObjectId.PartOfFortune
                    or AstrologicalObjectId.Ceres
                    or AstrologicalObjectId.Pallas
                    or AstrologicalObjectId.Juno
                    or AstrologicalObjectId.Vesta
            );

    private static string ObjectKey(
        AstrologicalObjectId id)
        =>
            id.ToString();

    private static SceneLayer SceneLayerFor(
        AstrologicalObjectId id)
        =>
            id switch
            {
                AstrologicalObjectId.Sun
                    or AstrologicalObjectId.Moon
                    or AstrologicalObjectId.Mercury
                    or AstrologicalObjectId.Venus
                    or AstrologicalObjectId.Mars
                    or AstrologicalObjectId.Jupiter
                    or AstrologicalObjectId.Saturn
                    or AstrologicalObjectId.Uranus
                    or AstrologicalObjectId.Neptune
                    or AstrologicalObjectId.Pluto =>
                        SceneLayer.BodyLayer,

                _ =>
                    SceneLayer.PointLayer
            };

    private static string GlyphKey(
        AstrologicalObjectId id)
        =>
            id switch
            {
                AstrologicalObjectId.Sun =>
                    "planet-sun",

                AstrologicalObjectId.Moon =>
                    "planet-moon",

                AstrologicalObjectId.Mercury =>
                    "planet-mercury",

                AstrologicalObjectId.Venus =>
                    "planet-venus",

                AstrologicalObjectId.Mars =>
                    "planet-mars",

                AstrologicalObjectId.Jupiter =>
                    "planet-jupiter",

                AstrologicalObjectId.Saturn =>
                    "planet-saturn",

                AstrologicalObjectId.Uranus =>
                    "planet-uranus",

                AstrologicalObjectId.Neptune =>
                    "planet-neptune",

                AstrologicalObjectId.Pluto =>
                    "planet-pluto",

                AstrologicalObjectId.NorthTrueNode =>
                    "point-north-node",

                AstrologicalObjectId.SouthNode =>
                    "point-south-node",

                AstrologicalObjectId.MeanLilith =>
                    "point-lilith",

                AstrologicalObjectId.PartOfFortune =>
                    "point-fortuna",

                AstrologicalObjectId.Chiron =>
                    "point-chiron",

                AstrologicalObjectId.Ceres =>
                    "asteroid-ceres",

                AstrologicalObjectId.Pallas =>
                    "asteroid-pallas",

                AstrologicalObjectId.Juno =>
                    "asteroid-juno",

                AstrologicalObjectId.Vesta =>
                    "asteroid-vesta",

                AstrologicalObjectId.Ascendant =>
                    "angle-asc",

                AstrologicalObjectId.Midheaven =>
                    "angle-mc",

                _ =>
                    throw new ArgumentOutOfRangeException(
                        nameof(id),
                        id,
                        "Unsupported graphics object.")
            };

    private static NatalAspectVisualClass AspectClass(
        AspectKind kind)
        =>
            kind switch
            {
                AspectKind.Conjunction
                    or AspectKind.Sextile
                    or AspectKind.Square
                    or AspectKind.Trine
                    or AspectKind.Opposition =>
                        NatalAspectVisualClass.Major,

                AspectKind.Semisextile
                    or AspectKind.Quincunx
                    or AspectKind.Quintile
                    or AspectKind.Biquintile =>
                        NatalAspectVisualClass.Secondary,

                _ =>
                    throw new ArgumentOutOfRangeException(
                        nameof(kind),
                        kind,
                        "Unsupported persisted aspect kind.")
            };

    private static string AspectKey(
        NatalAspectSnapshot aspect)
        =>
            string.Create(
                System.Globalization.CultureInfo.InvariantCulture,
                $"{ObjectKey(aspect.FirstObject)}-{ObjectKey(aspect.SecondObject)}-{aspect.Kind}");
}
