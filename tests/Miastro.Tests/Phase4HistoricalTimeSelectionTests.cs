using Miastro.Application.Time;
using Miastro.Domain.Geography;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4HistoricalTimeSelectionTests
{
    private readonly NodaTimeHistoricalTimeResolver _resolver = new();

    [TestMethod]
    public void AmbiguousResolution_DoesNotChooseSilently()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 10, 27, 2, 30),
            new IanaTimeZoneId("Europe/Madrid"));

        var snapshot =
            HistoricalTimeSelectionFactory.FromResolution(result);

        Assert.AreEqual(
            HistoricalTimeSelectionState.AmbiguousPendingChoice,
            snapshot.State);

        Assert.IsNull(snapshot.ChosenOffset);
        Assert.IsNull(snapshot.ChosenInstant);
        Assert.IsNull(snapshot.AmbiguityDecision);
    }

    [TestMethod]
    public void ExplicitAmbiguousChoice_IsAuditable()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 10, 27, 2, 30),
            new IanaTimeZoneId("Europe/Madrid"));

        var snapshot =
            HistoricalTimeSelectionFactory
                .ChooseAmbiguousCandidate(
                    result,
                    1,
                    "user-selected-later-offset");

        Assert.AreEqual(
            HistoricalTimeSelectionState.AmbiguousChosen,
            snapshot.State);

        Assert.IsNotNull(snapshot.ChosenOffset);
        Assert.IsNotNull(snapshot.ChosenInstant);

        Assert.AreEqual(
            "user-selected-later-offset",
            snapshot.AmbiguityDecision);
    }

    [TestMethod]
    public void SkippedTime_DoesNotInventInstant()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 3, 31, 2, 30),
            new IanaTimeZoneId("Europe/Madrid"));

        var snapshot =
            HistoricalTimeSelectionFactory.FromResolution(result);

        Assert.AreEqual(
            HistoricalTimeSelectionState.Skipped,
            snapshot.State);

        Assert.IsNull(snapshot.ChosenOffset);
        Assert.IsNull(snapshot.ChosenInstant);
    }
}
