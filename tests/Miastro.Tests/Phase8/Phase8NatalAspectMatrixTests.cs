using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalAspectMatrixTests
{
    [TestMethod]
    public void Participants_follow_canonical_order_and_v1_profile()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Midheaven),

                Placement(
                    AstrologicalObjectId.NorthTrueNode),

                Placement(
                    AstrologicalObjectId.Mars),

                Placement(
                    AstrologicalObjectId.Sun),

                Placement(
                    AstrologicalObjectId.Chiron)
            };

        var matrix =
            NatalAspectMatrixReader.Read(
                placements,
                Array.Empty<NatalAspectSnapshot>());

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Chiron,
                AstrologicalObjectId.Midheaven
            },
            matrix.Participants
                .Select(x => x.ObjectId)
                .ToArray());

        Assert.IsFalse(
            matrix.Participants.Any(
                x => x.ObjectId
                    == AstrologicalObjectId.NorthTrueNode));
    }

    [TestMethod]
    public void Matrix_is_strictly_triangular()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Sun),

                Placement(
                    AstrologicalObjectId.Moon),

                Placement(
                    AstrologicalObjectId.Mars),

                Placement(
                    AstrologicalObjectId.Saturn)
            };

        var matrix =
            NatalAspectMatrixReader.Read(
                placements,
                Array.Empty<NatalAspectSnapshot>());

        Assert.AreEqual(
            4,
            matrix.Participants.Count);

        Assert.AreEqual(
            6,
            matrix.Cells.Count);

        Assert.IsTrue(
            matrix.Cells.All(
                cell =>
                    cell.RowIndex
                    > cell.ColumnIndex));
    }

    [TestMethod]
    public void Pair_is_never_duplicated_as_a_b_and_b_a()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Moon),

                    Placement(
                        AstrologicalObjectId.Mars)
                },
                Array.Empty<NatalAspectSnapshot>());

        var normalizedPairs =
            matrix.Cells
                .Select(
                    cell =>
                        string.Join(
                            "|",
                            new[]
                            {
                                cell.RowObjectId
                                    .ToString(),

                                cell.ColumnObjectId
                                    .ToString()
                            }
                            .OrderBy(x => x)))
                .ToArray();

        Assert.AreEqual(
            normalizedPairs.Length,
            normalizedPairs
                .Distinct()
                .Count());
    }

    [TestMethod]
    public void Persisted_aspect_populates_correct_cell_without_recalculation()
    {
        var aspect =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Saturn,
                AspectKind.Square,
                SeparationDegrees: 92.233333,
                ExactAngleDegrees: 90.0,
                DeviationDegrees: 2.233333,
                AllowedOrbDegrees: 7.0,
                UsedOrbDegrees: 2.233333);

        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Saturn),

                    Placement(
                        AstrologicalObjectId.Sun)
                },
                new[]
                {
                    aspect
                });

        var cell =
            matrix.Cells.Single();

        Assert.IsTrue(
            cell.HasAspect);

        Assert.AreEqual(
            AspectKind.Square,
            cell.AspectKind);

        Assert.AreEqual(
            "Cuadratura",
            cell.AspectName);

        Assert.AreEqual(
            "□",
            cell.AspectSymbol);

        Assert.AreEqual(
            92.233333,
            cell.SeparationDegrees);

        Assert.AreEqual(
            2.233333,
            cell.UsedOrbDegrees);
    }

    [TestMethod]
    public void Absence_of_aspect_is_explicit()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Moon)
                },
                Array.Empty<NatalAspectSnapshot>());

        var cell =
            matrix.Cells.Single();

        Assert.IsFalse(
            cell.HasAspect);

        Assert.IsNull(
            cell.AspectKind);

        Assert.AreEqual(
            "Sin aspecto",
            cell.AspectName);

        Assert.AreEqual(
            string.Empty,
            cell.AspectSymbol);
    }

    [TestMethod]
    public void Unauthorized_persisted_participant_is_not_added_to_matrix()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Moon),

                    Placement(
                        AstrologicalObjectId.NorthTrueNode)
                },
                new[]
                {
                    new NatalAspectSnapshot(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.NorthTrueNode,
                        AspectKind.Sextile,
                        60.0,
                        60.0,
                        0.0,
                        5.0,
                        0.0)
                });

        Assert.IsFalse(
            matrix.Participants.Any(
                x => x.ObjectId
                    == AstrologicalObjectId.NorthTrueNode));

        Assert.AreEqual(
            1,
            matrix.Cells.Count);

        Assert.IsFalse(
            matrix.Cells.Single().HasAspect);
    }

    [TestMethod]
    public void Aspect_tooltip_facts_use_persisted_values()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Saturn)
                },
                new[]
                {
                    new NatalAspectSnapshot(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Saturn,
                        AspectKind.Square,
                        SeparationDegrees: 92.233333,
                        ExactAngleDegrees: 90.0,
                        DeviationDegrees: 2.233333,
                        AllowedOrbDegrees: 7.0,
                        UsedOrbDegrees: 2.233333)
                });

        var cell =
            matrix.Cells.Single();

        Assert.AreEqual(
            "92°14′",
            cell.SeparationText);

        Assert.AreEqual(
            "2°14′",
            cell.OrbText);

        Assert.AreEqual(
            "2°14′",
            cell.DeviationText);
    }

    [TestMethod]
    public void Accessible_name_does_not_depend_only_on_symbol_or_color()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Saturn)
                },
                new[]
                {
                    new NatalAspectSnapshot(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Saturn,
                        AspectKind.Square,
                        92.233333,
                        90.0,
                        2.233333,
                        7.0,
                        2.233333)
                });

        var name =
            matrix.Cells
                .Single()
                .AccessibleName;

        StringAssert.Contains(
            name,
            "Sol");

        StringAssert.Contains(
            name,
            "Saturno");

        StringAssert.Contains(
            name,
            "cuadratura");

        StringAssert.Contains(
            name,
            "orbe 2°14′");
    }

    [TestMethod]
    public void Reversed_persisted_pair_maps_to_same_canonical_cell()
    {
        var matrix =
            NatalAspectMatrixReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun),

                    Placement(
                        AstrologicalObjectId.Saturn)
                },
                new[]
                {
                    new NatalAspectSnapshot(
                        AstrologicalObjectId.Saturn,
                        AstrologicalObjectId.Sun,
                        AspectKind.Trine,
                        120.0,
                        120.0,
                        0.0,
                        7.0,
                        0.0)
                });

        var cell =
            matrix.Cells.Single();

        Assert.IsTrue(
            cell.HasAspect);

        Assert.AreEqual(
            AspectKind.Trine,
            cell.AspectKind);
    }

    [TestMethod]
    public void Duplicate_pair_is_rejected()
    {
        var duplicateRejected =
            false;

        try
        {
            _ =
                NatalAspectMatrixReader.Read(
                    new[]
                    {
                        Placement(
                            AstrologicalObjectId.Sun),

                        Placement(
                            AstrologicalObjectId.Saturn)
                    },
                    new[]
                    {
                        new NatalAspectSnapshot(
                            AstrologicalObjectId.Sun,
                            AstrologicalObjectId.Saturn,
                            AspectKind.Square,
                            90,
                            90,
                            0,
                            7,
                            0),

                        new NatalAspectSnapshot(
                            AstrologicalObjectId.Saturn,
                            AstrologicalObjectId.Sun,
                            AspectKind.Trine,
                            120,
                            120,
                            0,
                            7,
                            0)
                    });
        }
        catch (InvalidOperationException)
        {
            duplicateRejected =
                true;
        }

        Assert.IsTrue(
            duplicateRejected);
    }

    [TestMethod]
    public void Angle_formatter_is_culture_independent()
    {
        var previous =
            System.Globalization
                .CultureInfo.CurrentCulture;

        try
        {
            System.Globalization
                .CultureInfo.CurrentCulture =
                    System.Globalization
                        .CultureInfo.GetCultureInfo(
                            "en-US");

            Assert.AreEqual(
                "2°14′",
                NatalAspectAngleFormatter
                    .DegreesMinutes(
                        2.233333));
        }
        finally
        {
            System.Globalization
                .CultureInfo.CurrentCulture =
                    previous;
        }
    }

    private static NatalPlacementSnapshot Placement(
        AstrologicalObjectId objectId)
        => new(
            objectId,
            LongitudeDegrees: 0.0,
            LatitudeDegrees: null,
            DistanceAu: null,
            LongitudeSpeedDegreesPerDay: null,
            LatitudeSpeedDegreesPerDay: null,
            DistanceSpeedAuPerDay: null,
            Motion: MotionState.Direct,
            ZodiacSign: (int)ZodiacSign.Aries,
            DegreeInSign: 0.0,
            HouseNumber: 1);
}
