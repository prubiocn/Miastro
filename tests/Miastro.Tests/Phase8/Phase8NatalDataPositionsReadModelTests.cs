using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDataPositionsReadModelTests
{
    [TestMethod]
    public void Data_rows_use_canonical_order()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.Mars,
                    10.0,
                    ZodiacSign.Aries,
                    1),
                Placement(
                    AstrologicalObjectId.Sun,
                    40.0,
                    ZodiacSign.Taurus,
                    2));

        var rows =
            NatalDataPanelReader.Read(
                snapshot);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Mars
            },
            rows
                .Select(x => x.ObjectId)
                .ToArray());
    }

    [TestMethod]
    public void Data_row_contains_degree_sign_and_double_rulership()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.Mars,
                    217.25,
                    ZodiacSign.Scorpio,
                    8));

        var row =
            NatalDataPanelReader
                .Read(snapshot)
                .Single();

        Assert.AreEqual(
            "07° 15′",
            row.DegreeText);

        Assert.AreEqual(
            "Escorpio",
            row.SignName);

        Assert.AreEqual(
            "Marte / Plutón",
            row.SignRulersText);
    }

    [TestMethod]
    public void Positions_row_contains_exact_house_and_persisted_motion()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.Mercury,
                    103.5,
                    ZodiacSign.Cancer,
                    4,
                    MotionState.Retrograde));

        var row =
            NatalPositionsPanelReader
                .Read(snapshot)
                .Single();

        Assert.AreEqual(
            "13° 30′ Cáncer",
            row.ExactPositionText);

        Assert.AreEqual(
            "Casa 4",
            row.HouseText);

        Assert.AreEqual(
            "Retrógrado",
            row.MotionText);

        Assert.AreEqual(
            MotionState.Retrograde,
            row.Motion);
    }

    [TestMethod]
    public void Stationary_motion_is_not_inferred_away()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.Mercury,
                    103.5,
                    ZodiacSign.Cancer,
                    4,
                    MotionState.Stationary));

        var row =
            NatalPositionsPanelReader
                .Read(snapshot)
                .Single();

        Assert.AreEqual(
            "Estacionario",
            row.MotionText);

        Assert.AreEqual(
            MotionState.Stationary,
            row.Motion);
    }

    [TestMethod]
    public void Position_row_contains_sign_and_actual_house_rulers()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Mars,
                    213.0,
                    ZodiacSign.Scorpio,
                    7)
            };

        var cusps =
            StandardCusps()
                .Select(
                    cusp =>
                        cusp.HouseNumber == 7
                            ? new NatalHouseCuspSnapshot(
                                7,
                                35.0)
                            : cusp)
                .ToArray();

        var snapshot =
            Snapshot(
                placements,
                cusps);

        var row =
            NatalPositionsPanelReader
                .Read(snapshot)
                .Single();

        Assert.AreEqual(
            "Marte / Plutón",
            row.SignRulersText);

        Assert.AreEqual(
            "Tauro",
            row.HouseCuspSignText);

        Assert.AreEqual(
            "Venus",
            row.HouseRulersText);
    }

    [TestMethod]
    public void Ascendant_and_midheaven_are_flagged_as_angles()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.Midheaven,
                    280.0,
                    ZodiacSign.Capricorn,
                    10,
                    null),
                Placement(
                    AstrologicalObjectId.Ascendant,
                    100.0,
                    ZodiacSign.Cancer,
                    1,
                    null));

        var rows =
            NatalPositionsPanelReader.Read(
                snapshot);

        Assert.IsTrue(
            rows.Single(
                x => x.ObjectId
                    == AstrologicalObjectId.Ascendant)
                .IsAngle);

        Assert.IsTrue(
            rows.Single(
                x => x.ObjectId
                    == AstrologicalObjectId.Midheaven)
                .IsAngle);
    }

    [TestMethod]
    public void Additional_points_remain_available_to_positions_reader()
    {
        var snapshot =
            Snapshot(
                Placement(
                    AstrologicalObjectId.NorthTrueNode,
                    50.0,
                    ZodiacSign.Taurus,
                    2,
                    MotionState.Retrograde),
                Placement(
                    AstrologicalObjectId.MeanLilith,
                    80.0,
                    ZodiacSign.Gemini,
                    3),
                Placement(
                    AstrologicalObjectId.PartOfFortune,
                    120.0,
                    ZodiacSign.Leo,
                    5,
                    null),
                Placement(
                    AstrologicalObjectId.Ceres,
                    150.0,
                    ZodiacSign.Virgo,
                    6));

        var ids =
            NatalPositionsPanelReader
                .Read(snapshot)
                .Select(x => x.ObjectId)
                .ToArray();

        CollectionAssert.Contains(
            ids,
            AstrologicalObjectId.NorthTrueNode);

        CollectionAssert.Contains(
            ids,
            AstrologicalObjectId.MeanLilith);

        CollectionAssert.Contains(
            ids,
            AstrologicalObjectId.PartOfFortune);

        CollectionAssert.Contains(
            ids,
            AstrologicalObjectId.Ceres);
    }

    [TestMethod]
    public void Formatting_is_independent_from_current_culture()
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
                "29° 30′ Piscis",
                NatalPositionFormatter
                    .ExactPosition(
                        359.5));

            Assert.AreEqual(
                "00° 00′ Aries",
                NatalPositionFormatter
                    .ExactPosition(
                        360.0));
        }
        finally
        {
            System.Globalization
                .CultureInfo.CurrentCulture =
                    previous;
        }
    }

    [TestMethod]
    public void Minute_rounding_wraps_into_next_sign_deterministically()
    {
        Assert.AreEqual(
            "00° 00′ Tauro",
            NatalPositionFormatter
                .ExactPosition(
                    29.9999));
    }

    private static NatalChartSnapshotReadModel Snapshot(
        params NatalPlacementSnapshot[] placements)
        => Snapshot(
            placements,
            StandardCusps());

    private static NatalChartSnapshotReadModel Snapshot(
        IReadOnlyList<NatalPlacementSnapshot> placements,
        IReadOnlyList<NatalHouseCuspSnapshot> cusps)
        => new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NatalChartStatus.Current,
            "phase8-test-input-hash",
            false,
            new DateOnly(
                2000,
                1,
                1),
            new TimeOnly(
                12,
                0),
            new DateTimeOffset(
                2000,
                1,
                1,
                12,
                0,
                0,
                TimeSpan.Zero),
            "Localidad de prueba",
            0.0,
            0.0,
            "Etc/UTC",
            "test-tzdb",
            HouseSystem.Placidus,
            "MiastroV1",
            "phase8-test",
            "test-engine",
            "1.0",
            "1.0",
            "test-ephemeris",
            new DateTimeOffset(
                2000,
                1,
                1,
                12,
                0,
                1,
                TimeSpan.Zero),
            null,
            null,
            placements,
            cusps,
            Array.Empty<NatalAspectSnapshot>(),
            1,
            "phase8-test-birth-hash",
            0,
            0L,
            null,
            null);

    private static NatalPlacementSnapshot Placement(
        AstrologicalObjectId objectId,
        double longitude,
        ZodiacSign sign,
        int? house,
        MotionState? motion = MotionState.Direct)
        => new(
            objectId,
            longitude,
            null,
            null,
            null,
            null,
            null,
            motion,
            (int)sign,
            longitude % 30.0,
            house);

    private static IReadOnlyList<
        NatalHouseCuspSnapshot>
        StandardCusps()
        => Enumerable
            .Range(1, 12)
            .Select(
                house =>
                    new NatalHouseCuspSnapshot(
                        house,
                        (house - 1) * 30.0))
            .ToArray();
}
