using Miastro.Domain.Geography;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersonDomainTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 20, 18, 0, 0, TimeSpan.Zero);

    private static Latitude Lat() => new Latitude(40.0);
    private static Longitude Lon() => new Longitude(-3.0);
    private static IanaTimeZoneId Zone() => new IanaTimeZoneId("Europe/Madrid");

    [TestMethod]
    public void Persona_valid_tracks_creation()
    {
        var person = Persona.Create("Synthetic", "Person", Now);
        Assert.AreNotEqual(Guid.Empty, person.Id);
        Assert.AreEqual("Synthetic", person.FirstName);
        Assert.AreEqual("Person", person.LastName);
        Assert.AreEqual(Now, person.CreatedAtUtc);
        Assert.AreEqual(1, person.History.Count);
        Assert.AreEqual(PersonHistoryEventType.Created, person.History[0].EventType);
    }

    [TestMethod]
    public void Persona_requires_name()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => Persona.Create(" ", "Person", Now));
    }

    [TestMethod]
    public void Exact_and_approximate_require_concrete_time()
    {
        var exact = BirthData.CreateConcrete(
            new DateOnly(2000, 1, 1),
            BirthTimePrecision.Exact,
            new TimeOnly(12, 0),
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        var approximate = BirthData.CreateConcrete(
            new DateOnly(2000, 1, 1),
            BirthTimePrecision.Approximate,
            new TimeOnly(12, 0),
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        Assert.IsTrue(exact.RequiresConcreteTime);
        Assert.IsTrue(approximate.RequiresConcreteTime);
        Assert.AreEqual(BirthTemporalResolutionState.Pending, exact.ResolutionState);
        Assert.AreEqual(BirthTemporalResolutionState.Pending, approximate.ResolutionState);
    }

    [TestMethod]
    public void Range_is_persistable_without_single_instant()
    {
        var birth = BirthData.CreateRange(
            new DateOnly(2000, 1, 1),
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        Assert.AreEqual(BirthTimePrecision.Range, birth.TimePrecision);
        Assert.AreEqual(BirthTemporalResolutionState.NotApplicable, birth.ResolutionState);
        Assert.IsNull(birth.ResolvedInstantUtc);
    }

    [TestMethod]
    public void Invalid_range_is_rejected()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => BirthData.CreateRange(
                new DateOnly(2000, 1, 1),
                new TimeOnly(18, 0),
                new TimeOnly(9, 0),
                1,
                "Synthetic City",
                "Synthetic Country",
                "Synthetic Region",
                null,
                Lat(),
                Lon(),
                Zone()));
    }

    [TestMethod]
    public void Day_period_does_not_invent_exact_time()
    {
        var birth = BirthData.CreateDayPeriod(
            new DateOnly(2000, 1, 1),
            DayPeriod.Morning,
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        Assert.AreEqual(BirthTimePrecision.DayPeriod, birth.TimePrecision);
        Assert.AreEqual(DayPeriod.Morning, birth.DayPeriod);
        Assert.IsNull(birth.LocalTime);
        Assert.IsNull(birth.ResolvedInstantUtc);
    }

    [TestMethod]
    public void Unknown_time_does_not_invent_instant()
    {
        var birth = BirthData.CreateUnknown(
            new DateOnly(2000, 1, 1),
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        Assert.AreEqual(BirthTimePrecision.Unknown, birth.TimePrecision);
        Assert.IsNull(birth.LocalTime);
        Assert.IsNull(birth.ResolvedInstantUtc);
        Assert.AreEqual(BirthTemporalResolutionState.NotApplicable, birth.ResolutionState);
    }

    [TestMethod]
    public void Ambiguous_time_requires_explicit_selection()
    {
        var birth = BirthData.CreateConcrete(
            new DateOnly(2025, 10, 26),
            BirthTimePrecision.Exact,
            new TimeOnly(2, 30),
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            Lat(),
            Lon(),
            Zone());

        birth.MarkAmbiguous(
            "TZDB: test",
            7200,
            new DateTimeOffset(2025, 10, 26, 0, 30, 0, TimeSpan.Zero),
            3600,
            new DateTimeOffset(2025, 10, 26, 1, 30, 0, TimeSpan.Zero));

        Assert.IsNull(birth.ResolvedInstantUtc);
        Assert.IsNull(birth.AmbiguousSelectedCandidate);

        birth.SelectAmbiguousCandidate(2, Now);

        Assert.AreEqual(2, birth.AmbiguousSelectedCandidate);
        Assert.AreEqual(
            new DateTimeOffset(2025, 10, 26, 1, 30, 0, TimeSpan.Zero),
            birth.ResolvedInstantUtc);
    }

    [TestMethod]
    public void Favorite_and_consultation_have_distinct_semantics()
    {
        var person = Persona.Create("Synthetic", "Person", Now);
        person.SetFavorite(true, Now.AddMinutes(1));
        person.RecordConsultation(Now.AddMinutes(2));

        Assert.IsTrue(person.IsFavorite);
        Assert.AreEqual(Now.AddMinutes(2), person.LastConsultationAtUtc);
    }
}
