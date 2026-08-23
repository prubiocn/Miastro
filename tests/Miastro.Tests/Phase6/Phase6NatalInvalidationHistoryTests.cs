using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.Entities;
using Miastro.Infrastructure.Persistence.Natal;
using Miastro.Infrastructure.Persistence.People;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalInvalidationHistoryTests
{
    [TestMethod]
    public async Task Birth_change_invalidates_current_natal_atomically()
    {
        await using var fixture =
            await Fixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        await fixture.CreateNatalAsync(
            personId);

        var store =
            new EfPersonStore(
                fixture.Context);

        await store.UpdateAsync(
            Update(
                personId,
                localTime:
                    new TimeOnly(12, 15)),
            new DateTimeOffset(
                2026, 8, 21,
                12, 0, 0,
                TimeSpan.Zero));

        fixture.Context.ChangeTracker.Clear();

        var chart =
            await fixture.Context.NatalCharts
                .SingleAsync();

        Assert.AreEqual(
            (int)NatalChartStatus.Invalidated,
            chart.Status);

        Assert.IsNotNull(
            chart.InvalidatedAtUtc);

        var invalidationEvents =
            await fixture.Context.PersonHistory
                .CountAsync(x =>
                    x.PersonId == personId
                    && x.EventType ==
                        (int)PersonHistoryEventType
                            .NatalChartInvalidated);

        Assert.AreEqual(
            1,
            invalidationEvents);
    }

    [TestMethod]
    public async Task Contact_only_change_keeps_current_natal()
    {
        await using var fixture =
            await Fixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        await fixture.CreateNatalAsync(
            personId);

        var store =
            new EfPersonStore(
                fixture.Context);

        var command =
            Update(
                personId,
                localTime:
                    new TimeOnly(12, 0))
            with
            {
                Phone = "600 000 000"
            };

        await store.UpdateAsync(
            command,
            new DateTimeOffset(
                2026, 8, 21,
                12, 0, 0,
                TimeSpan.Zero));

        fixture.Context.ChangeTracker.Clear();

        var chart =
            await fixture.Context.NatalCharts
                .SingleAsync();

        Assert.AreEqual(
            (int)NatalChartStatus.Current,
            chart.Status);

        var invalidations =
            await fixture.Context.PersonHistory
                .CountAsync(x =>
                    x.PersonId == personId
                    && x.EventType ==
                        (int)PersonHistoryEventType
                            .NatalChartInvalidated);

        Assert.AreEqual(
            0,
            invalidations);
    }

    [TestMethod]
    public async Task First_snapshot_records_calculated_history()
    {
        await using var fixture =
            await Fixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        await fixture.CreateNatalAsync(
            personId);

        var events =
            await fixture.Context.PersonHistory
                .AsNoTracking()
                .Where(x =>
                    x.PersonId == personId)
                .ToListAsync();

        Assert.IsTrue(
            events.Any(x =>
                x.EventType ==
                    (int)PersonHistoryEventType
                        .NatalChartCalculated));
    }

    private static UpdatePersonCommand Update(
        Guid personId,
        TimeOnly localTime)
        => new(
            Id: personId,
            FirstName: "Persona",
            LastName: "Natal",
            Phone: null,
            Email: null,
            PrivateNote: null,
            IsFavorite: false,
            BirthData:
                Birth(
                    localTime),
            CurrentResidence:
                null);

    private static BirthDataWriteModel Birth(
        TimeOnly localTime)
        => new(
            LocalDate:
                new DateOnly(
                    2000, 1, 1),
            TimePrecision:
                BirthTimePrecision.Exact,
            LocalTime:
                localTime,
            RangeStart:
                null,
            RangeEnd:
                null,
            DayPeriod:
                null,
            GeoNameId:
                3117735,
            Locality:
                "Madrid",
            Country:
                "España",
            Region:
                "Madrid",
            Subregion:
                null,
            Latitude:
                40.4168,
            Longitude:
                -3.7038,
            IanaTimeZoneId:
                "Europe/Madrid",
            TzdbVersion:
                "TZDB: 2026c",
            ResolutionState:
                BirthTemporalResolutionState.Resolved,
            HistoricalOffsetSeconds:
                3600,
            ResolvedInstantUtc:
                new DateTimeOffset(
                    2000, 1, 1,
                    11, 0, 0,
                    TimeSpan.Zero),
            AmbiguousEarlierOffsetSeconds:
                null,
            AmbiguousEarlierInstantUtc:
                null,
            AmbiguousLaterOffsetSeconds:
                null,
            AmbiguousLaterInstantUtc:
                null,
            AmbiguousSelectedCandidate:
                null,
            AmbiguousSelectionRecordedAtUtc:
                null,
            ManualCoordinateOverride:
                false,
            OriginalGeoNamesLatitude:
                null,
            OriginalGeoNamesLongitude:
                null);

    private sealed class Fixture
        : IAsyncDisposable
    {
        public SqliteConnection Connection { get; }
        public MiastroDbContext Context { get; }

        private Fixture(
            SqliteConnection connection,
            MiastroDbContext context)
        {
            Connection = connection;
            Context = context;
        }

        public static async Task<Fixture>
            CreateAsync()
        {
            var connection =
                new SqliteConnection(
                    "Data Source=:memory:");

            await connection.OpenAsync();

            var options =
                new DbContextOptionsBuilder<
                    MiastroDbContext>()
                    .UseSqlite(
                        connection)
                    .Options;

            var context =
                new MiastroDbContext(
                    options);

            await context.Database
                .MigrateAsync();

            return new(
                connection,
                context);
        }

        public async Task<Guid>
            CreatePersonAsync()
        {
            var id =
                Guid.NewGuid();

            var now =
                new DateTimeOffset(
                    2026, 8, 21,
                    10, 0, 0,
                    TimeSpan.Zero);

            Context.People.Add(
                new PersonEntity
                {
                    Id = id,
                    FirstName = "Persona",
                    LastName = "Natal",
                    NormalizedName =
                        "persona natal",
                    IsFavorite = false,
                    CreatedAtUtc = now,
                    ModifiedAtUtc = now,
                    BirthData =
                        new BirthDataEntity
                        {
                            PersonId = id,
                            LocalDate =
                                new DateOnly(
                                    2000, 1, 1),
                            TimePrecision =
                                (int)BirthTimePrecision.Exact,
                            LocalTime =
                                new TimeOnly(
                                    12, 0),
                            GeoNameId =
                                3117735,
                            Locality =
                                "Madrid",
                            Country =
                                "España",
                            Region =
                                "Madrid",
                            Latitude =
                                40.4168,
                            Longitude =
                                -3.7038,
                            IanaTimeZoneId =
                                "Europe/Madrid",
                            TzdbVersion =
                                "TZDB: 2026c",
                            TemporalResolutionState =
                                (int)BirthTemporalResolutionState
                                    .Resolved,
                            HistoricalOffsetSeconds =
                                3600,
                            ResolvedInstantUtc =
                                new DateTimeOffset(
                                    2000, 1, 1,
                                    11, 0, 0,
                                    TimeSpan.Zero)
                        },
                    History =
                    [
                        new PersonHistoryEntity
                        {
                            PersonId = id,
                            EventType =
                                (int)PersonHistoryEventType.Created,
                            OccurredAtUtc =
                                now,
                            Summary =
                                "Persona creada"
                        }
                    ]
                });

            await Context.SaveChangesAsync();

            Context.ChangeTracker.Clear();

            return id;
        }

        public async Task CreateNatalAsync(
            Guid personId)
        {
            var store =
                new EfNatalChartStore(
                    Context);

            var snapshot =
                Phase6NatalTestSnapshotFactory.Create(
                    personId,
                    HouseSystem.Placidus);

            await store.SaveOrGetExistingAsync(
                snapshot,
                NatalInputHash.Compute(
                    snapshot.Input));

            Context.ChangeTracker.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
}
