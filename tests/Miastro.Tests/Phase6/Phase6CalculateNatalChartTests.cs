using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Domain.Angles;
using Miastro.Domain.Calculation;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6CalculateNatalChartTests
{
    [TestMethod]
    public async Task Exact_birth_calculates_complete_natal_chart()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Exact);

        var result =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                HouseSystem.Placidus,
                FixedNow);

        Assert.IsTrue(result.Success);

        Assert.AreEqual(
            NatalCalculationResultCode.Calculated,
            result.Code);

        Assert.IsNotNull(result.Chart);
        Assert.IsNotNull(result.Snapshot);

        Assert.AreEqual(
            ChartType.Natal,
            result.Chart.Type);

        Assert.AreEqual(
            21,
            result.Snapshot.Placements.Count);

        Assert.AreEqual(
            12,
            result.Snapshot.HouseCusps.Count);

        Assert.IsFalse(
            result.Snapshot.IsApproximateBirthTime);
    }

    [TestMethod]
    public async Task Approximate_birth_is_preserved_in_snapshot()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Approximate);

        var result =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                HouseSystem.Koch,
                FixedNow);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Snapshot);

        Assert.IsTrue(
            result.Snapshot.IsApproximateBirthTime);

        Assert.AreEqual(
            HouseSystem.Koch,
            result.Snapshot.HouseSystem);
    }

    [TestMethod]
    public async Task Unknown_birth_time_blocks_complete_chart()
    {
        var personId =
            Guid.NewGuid();

        var person =
            Person(
                personId,
                BirthTimePrecision.Unknown);

        var useCase =
            Build(
                person);

        var result =
            await useCase.ExecuteAsync(
                personId);

        Assert.IsFalse(result.Success);

        Assert.AreEqual(
            NatalCalculationResultCode.BirthTimeInsufficient,
            result.Code);

        Assert.IsNull(result.Chart);
        Assert.IsNull(result.Snapshot);
    }

    [TestMethod]
    public async Task South_node_is_derived_180_degrees_from_true_north_node()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Exact);

        var result =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                calculatedAtUtc: FixedNow);

        Assert.IsNotNull(result.Snapshot);

        var north =
            result.Snapshot.Placements
                .Single(x =>
                    x.ObjectId ==
                    AstrologicalObjectId.NorthTrueNode);

        var south =
            result.Snapshot.Placements
                .Single(x =>
                    x.ObjectId ==
                    AstrologicalObjectId.SouthNode);

        var expected =
            EclipticLongitude
                .FromDegrees(
                    north.LongitudeDegrees + 180.0)
                .Degrees;

        Assert.AreEqual(
            expected,
            south.LongitudeDegrees,
            1e-12);

        CollectionAssert.DoesNotContain(
            fixture.PositionCalculator.RequestedObjects,
            AstrologicalObjectId.SouthNode);
    }

    [TestMethod]
    public async Task Asc_mc_and_fortune_are_present_without_swiss_body_calls()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Exact);

        var result =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                calculatedAtUtc: FixedNow);

        Assert.IsNotNull(result.Snapshot);

        foreach (var objectId in new[]
        {
            AstrologicalObjectId.Ascendant,
            AstrologicalObjectId.Midheaven,
            AstrologicalObjectId.PartOfFortune
        })
        {
            Assert.IsTrue(
                result.Snapshot.Placements
                    .Any(x =>
                        x.ObjectId == objectId));
        }

        CollectionAssert.DoesNotContain(
            fixture.PositionCalculator.RequestedObjects,
            AstrologicalObjectId.Ascendant);

        CollectionAssert.DoesNotContain(
            fixture.PositionCalculator.RequestedObjects,
            AstrologicalObjectId.Midheaven);

        CollectionAssert.DoesNotContain(
            fixture.PositionCalculator.RequestedObjects,
            AstrologicalObjectId.PartOfFortune);
    }

    [TestMethod]
    public async Task Same_inputs_return_current_snapshot_without_recalculation()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Exact);

        var first =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                calculatedAtUtc: FixedNow);

        var callsAfterFirst =
            fixture.PositionCalculator
                .RequestedObjects.Count;

        var second =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                calculatedAtUtc: FixedNow.AddMinutes(1));

        Assert.AreEqual(
            NatalCalculationResultCode
                .ExistingCurrentSnapshot,
            second.Code);

        Assert.AreEqual(
            callsAfterFirst,
            fixture.PositionCalculator
                .RequestedObjects.Count);

        Assert.AreEqual(
            first.Snapshot!.Id,
            second.Snapshot!.Id);
    }

    [TestMethod]
    public async Task Both_house_systems_produce_different_input_hashes()
    {
        var fixture =
            CreateFixture(
                BirthTimePrecision.Exact);

        var placidus =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                HouseSystem.Placidus,
                FixedNow);

        var koch =
            await fixture.UseCase.ExecuteAsync(
                fixture.PersonId,
                HouseSystem.Koch,
                FixedNow.AddMinutes(1));

        Assert.IsNotNull(placidus.Snapshot);
        Assert.IsNotNull(koch.Snapshot);

        Assert.AreNotEqual(
            placidus.Snapshot.InputHash,
            koch.Snapshot.InputHash);
    }

    private static readonly DateTimeOffset FixedNow =
        new(
            2026, 8, 21,
            12, 0, 0,
            TimeSpan.Zero);

    private static Fixture CreateFixture(
        BirthTimePrecision precision)
    {
        var personId =
            Guid.NewGuid();

        var person =
            Person(
                personId,
                precision);

        var positionCalculator =
            new FakePositionCalculator();

        var store =
            new MemoryNatalStore();

        var useCase =
            Build(
                person,
                positionCalculator,
                store);

        return new(
            personId,
            useCase,
            positionCalculator);
    }

    private static CalculateNatalChartUseCase Build(
        PersonDetails person,
        FakePositionCalculator? positionCalculator = null,
        MemoryNatalStore? store = null)
    {
        positionCalculator ??=
            new FakePositionCalculator();

        store ??=
            new MemoryNatalStore();

        return new(
            new SinglePersonStore(person),
            store,
            positionCalculator,
            new FakeHouseCalculator(),
            new FakeMetadataProvider());
    }

    private static PersonDetails Person(
        Guid id,
        BirthTimePrecision precision)
    {
        BirthDataReadModel birth;

        if (precision ==
            BirthTimePrecision.Unknown)
        {
            birth = new(
                LocalDate: new DateOnly(2000, 1, 1),
                TimePrecision: precision,
                LocalTime: null,
                RangeStart: null,
                RangeEnd: null,
                DayPeriod: null,
                GeoNameId: 3117735,
                Locality: "Madrid",
                Country: "España",
                Region: "Madrid",
                Subregion: null,
                Latitude: 40.4168,
                Longitude: -3.7038,
                IanaTimeZoneId: "Europe/Madrid",
                TzdbVersion: null,
                ResolutionState:
                    BirthTemporalResolutionState.NotApplicable,
                HistoricalOffsetSeconds: null,
                ResolvedInstantUtc: null,
                AmbiguousEarlierOffsetSeconds: null,
                AmbiguousEarlierInstantUtc: null,
                AmbiguousLaterOffsetSeconds: null,
                AmbiguousLaterInstantUtc: null,
                AmbiguousSelectedCandidate: null,
                AmbiguousSelectionRecordedAtUtc: null,
                ManualCoordinateOverride: false,
                OriginalGeoNamesLatitude: null,
                OriginalGeoNamesLongitude: null);
        }
        else
        {
            birth = new(
                LocalDate: new DateOnly(2000, 1, 1),
                TimePrecision: precision,
                LocalTime: new TimeOnly(12, 0),
                RangeStart: null,
                RangeEnd: null,
                DayPeriod: null,
                GeoNameId: 3117735,
                Locality: "Madrid",
                Country: "España",
                Region: "Madrid",
                Subregion: null,
                Latitude: 40.4168,
                Longitude: -3.7038,
                IanaTimeZoneId: "Europe/Madrid",
                TzdbVersion: "TZDB: 2026c",
                ResolutionState:
                    BirthTemporalResolutionState.Resolved,
                HistoricalOffsetSeconds: 3600,
                ResolvedInstantUtc:
                    new DateTimeOffset(
                        2000, 1, 1,
                        11, 0, 0,
                        TimeSpan.Zero),
                AmbiguousEarlierOffsetSeconds: null,
                AmbiguousEarlierInstantUtc: null,
                AmbiguousLaterOffsetSeconds: null,
                AmbiguousLaterInstantUtc: null,
                AmbiguousSelectedCandidate: null,
                AmbiguousSelectionRecordedAtUtc: null,
                ManualCoordinateOverride: false,
                OriginalGeoNamesLatitude: null,
                OriginalGeoNamesLongitude: null);
        }

        return new(
            id,
            "Persona",
            "Natal",
            null,
            null,
            null,
            false,
            FixedNow,
            FixedNow,
            null,
            birth,
            null,
            []);
    }

    private sealed record Fixture(
        Guid PersonId,
        CalculateNatalChartUseCase UseCase,
        FakePositionCalculator PositionCalculator);

    private sealed class FakeMetadataProvider
        : INatalCalculationMetadataProvider
    {
        public NatalCalculationEnvironment Get()
            => new(
                "0.6.0-phase6",
                "Swiss Ephemeris",
                "2.10.03",
                "1.0.0",
                "phase3-ephemeris-1800-2399");
    }

    private sealed class FakePositionCalculator
        : IEclipticPositionCalculator
    {
        public List<AstrologicalObjectId>
            RequestedObjects { get; } = [];

        public EclipticPosition Calculate(
            AstrologicalObjectId objectId,
            AstronomicalInstant instant,
            CalculationProfile profile)
        {
            RequestedObjects.Add(objectId);

            var longitude =
                objectId switch
                {
                    AstrologicalObjectId.Sun => 200.0,
                    AstrologicalObjectId.Moon => 260.0,
                    AstrologicalObjectId.Mercury => 210.0,
                    AstrologicalObjectId.Venus => 180.0,
                    AstrologicalObjectId.Mars => 90.0,
                    AstrologicalObjectId.Jupiter => 30.0,
                    AstrologicalObjectId.Saturn => 300.0,
                    AstrologicalObjectId.Uranus => 315.0,
                    AstrologicalObjectId.Neptune => 305.0,
                    AstrologicalObjectId.Pluto => 250.0,
                    AstrologicalObjectId.NorthTrueNode => 120.0,
                    AstrologicalObjectId.MeanLilith => 150.0,
                    AstrologicalObjectId.Chiron => 240.0,
                    AstrologicalObjectId.Ceres => 45.0,
                    AstrologicalObjectId.Pallas => 75.0,
                    AstrologicalObjectId.Juno => 105.0,
                    AstrologicalObjectId.Vesta => 135.0,
                    _ => throw new ArgumentOutOfRangeException()
                };

            return new(
                objectId,
                EclipticLongitude
                    .FromDegrees(longitude),
                0.25,
                1.0,
                objectId ==
                    AstrologicalObjectId.Saturn
                        ? -0.05
                        : 0.5,
                0.01,
                0.001,
                instant,
                ReferenceFrame.Geocentric,
                ["SWIEPH", "SPEED"],
                new AstronomyEngineMetadata(
                    "Swiss Ephemeris",
                    "2.10.03",
                    "1.0.0",
                    "X64"));
        }
    }

    private sealed class FakeHouseCalculator
        : IHouseCalculator
    {
        public HouseCalculationResult Calculate(
            AstronomicalInstant instant,
            GeographicLocation location,
            HouseSystem houseSystem)
        {
            var cusps =
                Enumerable.Range(1, 12)
                    .Select(number =>
                        new HouseCusp(
                            AstrologicalHouse
                                .FromNumber(number),
                            EclipticLongitude
                                .FromDegrees(
                                    (number - 1) * 30.0)))
                    .ToArray();

            return HouseCalculationResult
                .Succeeded(
                    houseSystem,
                    cusps,
                    EclipticLongitude
                        .FromDegrees(0.0),
                    EclipticLongitude
                        .FromDegrees(270.0),
                    location,
                    instant,
                    new AstronomyEngineMetadata(
                        "Swiss Ephemeris",
                        "2.10.03",
                        "1.0.0",
                        "X64"));
        }
    }

    private sealed class MemoryNatalStore
        : INatalChartStore
    {
        private readonly List<
            NatalChartSnapshotReadModel> _charts = [];

        public Task<PersistNatalChartResult>
            SaveOrGetExistingAsync(
                NatalChartSnapshotWriteModel snapshot,
                string inputHash,
                CancellationToken cancellationToken = default)
        {
            var existing =
                _charts.SingleOrDefault(x =>
                    x.PersonId == snapshot.PersonId
                    && x.InputHash == inputHash);

            if (existing is not null)
            {
                return Task.FromResult(
                    new PersistNatalChartResult(
                        existing,
                        false));
            }

            for (var i = 0; i < _charts.Count; i++)
            {
                if (_charts[i].PersonId ==
                    snapshot.PersonId
                    && _charts[i].Status ==
                    NatalChartStatus.Current)
                {
                    _charts[i] =
                        _charts[i] with
                        {
                            Status =
                                NatalChartStatus.Superseded
                        };
                }
            }

            var id =
                Guid.NewGuid();

            var chart =
                new NatalChartSnapshotReadModel(
                    id,
                    snapshot.PersonId,
                    NatalChartStatus.Current,
                    inputHash,
                    snapshot.IsApproximateBirthTime,
                    snapshot.Input.LocalDate,
                    snapshot.Input.LocalTime,
                    snapshot.Input.InstantUtc,
                    snapshot.Locality,
                    snapshot.Input.Latitude,
                    snapshot.Input.Longitude,
                    snapshot.Input.IanaTimeZoneId,
                    snapshot.Input.TzdbVersion,
                    snapshot.Input.HouseSystem,
                    snapshot.Input.CalculationProfileId,
                    snapshot.MiastroVersion,
                    snapshot.Input.Engine,
                    snapshot.Input.EngineVersion,
                    snapshot.AdapterVersion,
                    snapshot.Input.EphemerisVersion,
                    snapshot.CalculatedAtUtc,
                    null,
                    null,
                    snapshot.Placements,
                    snapshot.HouseCusps,
                    snapshot.Aspects);

            _charts.Add(chart);

            return Task.FromResult(
                new PersistNatalChartResult(
                    chart,
                    true));
        }

        public Task<NatalChartSnapshotReadModel?>
            GetCurrentAsync(
                Guid personId,
                CancellationToken cancellationToken = default)
            => Task.FromResult(
                _charts
                    .LastOrDefault(x =>
                        x.PersonId == personId
                        && x.Status ==
                            NatalChartStatus.Current));

        public Task<NatalChartSnapshotReadModel?>
            GetByInputHashAsync(
                Guid personId,
                string inputHash,
                CancellationToken cancellationToken = default)
            => Task.FromResult(
                _charts
                    .SingleOrDefault(x =>
                        x.PersonId == personId
                        && x.InputHash == inputHash));

        public Task InvalidateCurrentAsync(
            Guid personId,
            DateTimeOffset invalidatedAtUtc,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class SinglePersonStore(
        PersonDetails person)
        : IPersonStore
    {
        public Task<PersonDetails?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            => Task.FromResult(
                id == person.Id
                    ? person
                    : null);

        public Task<Guid> CreateAsync(
            CreatePersonCommand command,
            DateTimeOffset nowUtc,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task UpdateAsync(
            UpdatePersonCommand command,
            DateTimeOffset nowUtc,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<PersonListItem>>
            SearchAsync(
                PersonSearchQuery query,
                CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task SetFavoriteAsync(
            Guid id,
            bool isFavorite,
            DateTimeOffset nowUtc,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task RecordConsultationAsync(
            Guid id,
            DateTimeOffset nowUtc,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }
}
