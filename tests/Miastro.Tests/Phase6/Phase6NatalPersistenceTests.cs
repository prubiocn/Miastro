using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.Entities;
using Miastro.Infrastructure.Persistence.Natal;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalPersistenceTests
{
    [TestMethod]
    public async Task Phase6_migration_creates_normalized_natal_tables()
    {
        await using var fixture =
            await DatabaseFixture.CreateAsync();

        var tables =
            await ReadTablesAsync(
                fixture.Connection);

        CollectionAssert.IsSubsetOf(
            new[]
            {
                "NatalCharts",
                "NatalPlacements",
                "NatalHouseCusps",
                "NatalAspects"
            },
            tables.ToArray());
    }

    [TestMethod]
    public async Task Snapshot_roundtrips_with_children()
    {
        await using var fixture =
            await DatabaseFixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        var store =
            new EfNatalChartStore(
                fixture.Context);

        var snapshot =
            Snapshot(
                personId,
                HouseSystem.Placidus);

        var hash =
            NatalInputHash.Compute(
                snapshot.Input);

        var saved =
            await store.SaveOrGetExistingAsync(
                snapshot,
                hash);

        Assert.IsTrue(saved.Created);

        fixture.Context.ChangeTracker.Clear();

        var loaded =
            await store.GetCurrentAsync(
                personId);

        Assert.IsNotNull(loaded);

        Assert.AreEqual(
            saved.Chart.Id,
            loaded.Id);

        Assert.AreEqual(
            21,
            loaded.Placements.Count);

        Assert.AreEqual(
            12,
            loaded.HouseCusps.Count);

        Assert.AreEqual(
            0,
            loaded.Aspects.Count);

        Assert.AreEqual(
            hash,
            loaded.InputHash);
    }

    [TestMethod]
    public async Task Identical_input_hash_is_idempotent()
    {
        await using var fixture =
            await DatabaseFixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        var useCase =
            new PersistNatalChartSnapshotUseCase(
                new EfNatalChartStore(
                    fixture.Context));

        var snapshot =
            Snapshot(
                personId,
                HouseSystem.Placidus);

        var first =
            await useCase.ExecuteAsync(
                snapshot);

        fixture.Context.ChangeTracker.Clear();

        var second =
            await useCase.ExecuteAsync(
                snapshot);

        Assert.IsTrue(first.Created);
        Assert.IsFalse(second.Created);

        Assert.AreEqual(
            first.Chart.Id,
            second.Chart.Id);

        Assert.AreEqual(
            1,
            await fixture.Context.NatalCharts
                .CountAsync());
    }

    [TestMethod]
    public async Task New_input_supersedes_previous_current_chart()
    {
        await using var fixture =
            await DatabaseFixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        var useCase =
            new PersistNatalChartSnapshotUseCase(
                new EfNatalChartStore(
                    fixture.Context));

        var first =
            await useCase.ExecuteAsync(
                Snapshot(
                    personId,
                    HouseSystem.Placidus));

        fixture.Context.ChangeTracker.Clear();

        var second =
            await useCase.ExecuteAsync(
                Snapshot(
                    personId,
                    HouseSystem.Koch));

        Assert.IsTrue(first.Created);
        Assert.IsTrue(second.Created);

        fixture.Context.ChangeTracker.Clear();

        var firstEntity =
            await fixture.Context.NatalCharts
                .SingleAsync(x =>
                    x.Id == first.Chart.Id);

        var current =
            await fixture.Context.NatalCharts
                .SingleAsync(x =>
                    x.Status ==
                        (int)NatalChartStatus.Current);

        Assert.AreEqual(
            (int)NatalChartStatus.Superseded,
            firstEntity.Status);

        Assert.AreEqual(
            second.Chart.Id,
            firstEntity.SupersededByChartId);

        Assert.AreEqual(
            second.Chart.Id,
            current.Id);
    }

    [TestMethod]
    public async Task Current_chart_can_be_invalidated()
    {
        await using var fixture =
            await DatabaseFixture.CreateAsync();

        var personId =
            await fixture.CreatePersonAsync();

        var store =
            new EfNatalChartStore(
                fixture.Context);

        var snapshot =
            Snapshot(
                personId,
                HouseSystem.Placidus);

        await store.SaveOrGetExistingAsync(
            snapshot,
            NatalInputHash.Compute(
                snapshot.Input));

        var invalidatedAt =
            new DateTimeOffset(
                2026, 8, 21,
                12, 0, 0,
                TimeSpan.Zero);

        await store.InvalidateCurrentAsync(
            personId,
            invalidatedAt);

        fixture.Context.ChangeTracker.Clear();

        var chart =
            await fixture.Context.NatalCharts
                .SingleAsync();

        Assert.AreEqual(
            (int)NatalChartStatus.Invalidated,
            chart.Status);

        Assert.AreEqual(
            invalidatedAt,
            chart.InvalidatedAtUtc);

        Assert.IsNull(
            await store.GetCurrentAsync(
                personId));
    }

    private static NatalChartSnapshotWriteModel Snapshot(
        Guid personId,
        HouseSystem houseSystem)
        => Phase6NatalTestSnapshotFactory.Create(
            personId,
            houseSystem);

    private static async Task<HashSet<string>>
        ReadTablesAsync(
            SqliteConnection connection)
    {
        var result =
            new HashSet<string>(
                StringComparer.Ordinal);

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT name
            FROM sqlite_master
            WHERE type='table';
            """;

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(
                reader.GetString(0));
        }

        return result;
    }

    private sealed class DatabaseFixture
        : IAsyncDisposable
    {
        public SqliteConnection Connection { get; }

        public MiastroDbContext Context { get; }

        private DatabaseFixture(
            SqliteConnection connection,
            MiastroDbContext context)
        {
            Connection = connection;
            Context = context;
        }

        public static async Task<DatabaseFixture>
            CreateAsync()
        {
            var connection =
                new SqliteConnection(
                    "Data Source=:memory:");

            await connection.OpenAsync();

            var options =
                new DbContextOptionsBuilder<MiastroDbContext>()
                    .UseSqlite(connection)
                    .Options;

            var context =
                new MiastroDbContext(
                    options);

            await context.Database.MigrateAsync();

            return new(
                connection,
                context);
        }

        public async Task<Guid> CreatePersonAsync()
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
                    NormalizedName = "persona natal",
                    IsFavorite = false,
                    CreatedAtUtc = now,
                    ModifiedAtUtc = now
                });

            await Context.SaveChangesAsync();

            Context.ChangeTracker.Clear();

            var persisted =
                await Context.People
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == id);

            Assert.IsTrue(
                persisted,
                "La Persona fixture debe existir antes de guardar la carta.");

            return id;
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
    [TestMethod]
    public async Task Store_rejects_input_hash_that_does_not_match_snapshot()
    {
        await using var database =
            await DatabaseFixture.CreateAsync();

        var store =
            new EfNatalChartStore(
                database.Context);

        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var invalidHash =
            new string(
                '0',
                64);

        var expectedHash =
            NatalInputHash.Compute(
                snapshot.Input);

        Assert.AreNotEqual(
            expectedHash,
            invalidHash);

        await Assert.ThrowsExactlyAsync<
            ArgumentException>(
                async () =>
                    await store
                        .SaveOrGetExistingAsync(
                            snapshot,
                            invalidHash));
    }


}
