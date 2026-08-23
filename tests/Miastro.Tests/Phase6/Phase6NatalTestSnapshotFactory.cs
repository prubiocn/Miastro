using Miastro.Application.Natal;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.People;
using Miastro.Domain.Placements;

namespace Miastro.Tests.Phase6;

internal static class Phase6NatalTestSnapshotFactory
{
    public static NatalChartSnapshotWriteModel Create(
        Guid personId,
        HouseSystem houseSystem = HouseSystem.Placidus)
    {
        var input =
            new NatalInputFingerprint(
                new DateOnly(
                    2000, 1, 1),
                new TimeOnly(
                    12, 0),
                new DateTimeOffset(
                    2000, 1, 1,
                    11, 0, 0,
                    TimeSpan.Zero),
                40.4168,
                -3.7038,
                "Europe/Madrid",
                "TZDB: 2026c",
                houseSystem,
                "miastro-v1",
                "Swiss Ephemeris",
                "2.10.03",
                "test-ephemeris",
                BirthTimePrecision.Exact,
                3117735,
                "Madrid",
                3600,
                null);

        var longitudes =
            new Dictionary<
                AstrologicalObjectId,
                double>
            {
                [AstrologicalObjectId.Sun] = 10.0,
                [AstrologicalObjectId.Moon] = 20.0,
                [AstrologicalObjectId.Mercury] = 30.0,
                [AstrologicalObjectId.Venus] = 40.0,
                [AstrologicalObjectId.Mars] = 50.0,
                [AstrologicalObjectId.Jupiter] = 60.0,
                [AstrologicalObjectId.Saturn] = 70.0,
                [AstrologicalObjectId.Uranus] = 80.0,
                [AstrologicalObjectId.Neptune] = 90.0,
                [AstrologicalObjectId.Pluto] = 100.0,

                [AstrologicalObjectId.NorthTrueNode] =
                    120.0,

                [AstrologicalObjectId.SouthNode] =
                    300.0,

                [AstrologicalObjectId.MeanLilith] =
                    140.0,

                [AstrologicalObjectId.PartOfFortune] =
                    150.0,

                [AstrologicalObjectId.Chiron] = 160.0,
                [AstrologicalObjectId.Ceres] = 170.0,
                [AstrologicalObjectId.Pallas] = 180.0,
                [AstrologicalObjectId.Juno] = 190.0,
                [AstrologicalObjectId.Vesta] = 200.0,

                [AstrologicalObjectId.Ascendant] =
                    210.0,

                [AstrologicalObjectId.Midheaven] =
                    220.0
            };

        var placements =
            NatalObjectOrder.All
                .Select(objectId =>
                {
                    var longitude =
                        longitudes[objectId];

                    return new NatalPlacementSnapshot(
                        objectId,
                        longitude,
                        0.0,
                        1.0,
                        objectId is
                            AstrologicalObjectId.Ascendant
                            or AstrologicalObjectId.Midheaven
                            or AstrologicalObjectId.PartOfFortune
                                ? null
                                : 0.5,
                        0.0,
                        0.0,
                        objectId is
                            AstrologicalObjectId.Ascendant
                            or AstrologicalObjectId.Midheaven
                            or AstrologicalObjectId.PartOfFortune
                                ? null
                                : MotionState.Direct,
                        (int)Math.Floor(
                            longitude / 30.0),
                        longitude % 30.0,
                        ((int)Math.Floor(
                            longitude / 30.0) % 12) + 1);
                })
                .ToArray();

        var cusps =
            Enumerable.Range(1, 12)
                .Select(number =>
                    new NatalHouseCuspSnapshot(
                        number,
                        (number - 1) * 30.0))
                .ToArray();

        return new(
            personId,
            input,
            IsApproximateBirthTime: false,
            Locality: "Madrid",
            MiastroVersion: "0.6-test",
            AdapterVersion: "test-adapter",
            CalculatedAtUtc:
                new DateTimeOffset(
                    2026, 8, 21,
                    11, 0, 0,
                    TimeSpan.Zero),
            Placements:
                placements,
            HouseCusps:
                cusps,
            Aspects:
                Array.Empty<NatalAspectSnapshot>(),
            BirthDataVersion:
                NatalBirthDataIdentity.CurrentVersion,
            BirthDataHash:
                NatalBirthDataIdentity.Compute(
                    input));
    }
}
