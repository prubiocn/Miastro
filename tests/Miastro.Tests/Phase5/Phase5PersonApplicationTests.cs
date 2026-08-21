using Microsoft.EntityFrameworkCore;
using Miastro.Application.People;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.People;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersonApplicationTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 20, 18, 30, 0, TimeSpan.Zero);

    [TestMethod]
    public async Task Create_get_search_favorite_recent_and_delete_work()
    {
        var path = TempDatabase();

        try
        {
            await using var db = CreateContext(path);
            await db.Database.MigrateAsync();

            var store = new EfPersonStore(db);
            var create = new CreatePersonUseCase(store);

            var id = await create.ExecuteAsync(
                Command("Synthetic", "Alpha"),
                Now);

            var get = new GetPersonUseCase(store);
            var details = await get.ExecuteAsync(id);

            Assert.IsNotNull(details);
            Assert.AreEqual("Synthetic", details.FirstName);

            var search = new SearchPeopleUseCase(store);

            var matches = await search.ExecuteAsync(
                new PersonSearchQuery(
                    "synt",
                    PersonFilter.All,
                    PersonSort.FirstName));

            Assert.AreEqual(1, matches.Count);

            var favorite = new SetFavoriteUseCase(store);

            await favorite.ExecuteAsync(
                id,
                true,
                Now.AddMinutes(1));

            var favorites = await search.ExecuteAsync(
                new PersonSearchQuery(
                    null,
                    PersonFilter.Favorites,
                    PersonSort.Favorite));

            Assert.AreEqual(1, favorites.Count);
            Assert.IsTrue(favorites[0].IsFavorite);

            var consultation =
                new RecordPersonConsultationUseCase(store);

            await consultation.ExecuteAsync(
                id,
                Now.AddMinutes(2));

            var recent = await search.ExecuteAsync(
                new PersonSearchQuery(
                    null,
                    PersonFilter.Recent,
                    PersonSort.LastConsultation));

            Assert.AreEqual(1, recent.Count);
            Assert.IsNotNull(
                recent[0].LastConsultationAtUtc);

            var delete = new DeletePersonUseCase(store);

            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () => delete.ExecuteAsync(
                    id,
                    confirmed: false));

            await delete.ExecuteAsync(
                id,
                confirmed: true);

            Assert.IsNull(
                await get.ExecuteAsync(id));
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Search_order_is_stable()
    {
        var path = TempDatabase();

        try
        {
            await using var db = CreateContext(path);
            await db.Database.MigrateAsync();

            var store = new EfPersonStore(db);
            var create = new CreatePersonUseCase(store);

            _ = await create.ExecuteAsync(
                Command("Beta", "Zulu"),
                Now);

            _ = await create.ExecuteAsync(
                Command("Alpha", "Zulu"),
                Now.AddSeconds(1));

            var search = new SearchPeopleUseCase(store);

            var people = await search.ExecuteAsync(
                new PersonSearchQuery(
                    null,
                    PersonFilter.All,
                    PersonSort.FirstName));

            Assert.AreEqual(2, people.Count);
            Assert.AreEqual("Alpha", people[0].FirstName);
            Assert.AreEqual("Beta", people[1].FirstName);
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public void Unknown_birth_time_with_instant_is_rejected()
    {
        var invalidBirth = new BirthDataWriteModel(
            new DateOnly(2000, 1, 1),
            BirthTimePrecision.Unknown,
            null,
            null,
            null,
            null,
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            40,
            -3,
            "Europe/Madrid",
            null,
            BirthTemporalResolutionState.NotApplicable,
            null,
            DateTimeOffset.UtcNow,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            null);

        Assert.ThrowsExactly<ArgumentException>(
            () => PersonInputValidator.Validate(
                new CreatePersonCommand(
                    "Synthetic",
                    "Person",
                    null,
                    null,
                    null,
                    false,
                    invalidBirth,
                    null)));
    }

    [TestMethod]
    public void Ambiguous_birth_without_choice_is_rejected()
    {
        var ambiguous = new BirthDataWriteModel(
            new DateOnly(2025, 10, 26),
            BirthTimePrecision.Exact,
            new TimeOnly(2, 30),
            null,
            null,
            null,
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            40,
            -3,
            "Europe/Madrid",
            "TZDB:test",
            BirthTemporalResolutionState.Ambiguous,
            null,
            null,
            7200,
            new DateTimeOffset(
                2025, 10, 26, 0, 30, 0, TimeSpan.Zero),
            3600,
            new DateTimeOffset(
                2025, 10, 26, 1, 30, 0, TimeSpan.Zero),
            null,
            null,
            false,
            null,
            null);

        Assert.ThrowsExactly<ArgumentException>(
            () => PersonInputValidator.Validate(
                new CreatePersonCommand(
                    "Synthetic",
                    "Person",
                    null,
                    null,
                    null,
                    false,
                    ambiguous,
                    null)));
    }

    private static CreatePersonCommand Command(
        string firstName,
        string lastName)
        => new(
            firstName,
            lastName,
            null,
            null,
            null,
            false,
            null,
            null);

    private static MiastroDbContext CreateContext(
        string path)
    {
        var options =
            new DbContextOptionsBuilder<MiastroDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

        return new MiastroDbContext(options);
    }

    private static string TempDatabase()
        => Path.Combine(
            Path.GetTempPath(),
            $"miastro-phase5-app-{Guid.NewGuid():N}.sqlite");

    private static void DeleteDatabase(
        string path)
    {
        foreach (var candidate in new[]
        {
            path,
            path + "-wal",
            path + "-shm"
        })
        {
            if (File.Exists(candidate))
            {
                File.Delete(candidate);
            }
        }
    }
}
