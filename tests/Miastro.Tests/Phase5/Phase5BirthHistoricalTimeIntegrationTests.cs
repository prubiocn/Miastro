using Miastro.Application.People;
using Miastro.Application.Time;
using Miastro.Infrastructure.Time.Historical;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5BirthHistoricalTimeIntegrationTests
{
    [TestMethod]
    public void Normal_birth_time_resolves()
    {
        var useCase = new ResolveBirthHistoricalTimeUseCase(
            new NodaTimeHistoricalTimeResolver());

        var result = useCase.Execute(
            new DateOnly(2025, 1, 15),
            new TimeOnly(12, 0),
            "Europe/Madrid");

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Resolved,
            result.Resolution.Status);
    }

    [TestMethod]
    public void Ambiguous_birth_time_returns_two_candidates()
    {
        var useCase = new ResolveBirthHistoricalTimeUseCase(
            new NodaTimeHistoricalTimeResolver());

        var result = useCase.Execute(
            new DateOnly(2025, 10, 26),
            new TimeOnly(2, 30),
            "Europe/Madrid");

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Ambiguous,
            result.Resolution.Status);

        Assert.AreEqual(
            2,
            result.Resolution.Candidates.Count);
    }

    [TestMethod]
    public void Skipped_birth_time_does_not_resolve()
    {
        var useCase = new ResolveBirthHistoricalTimeUseCase(
            new NodaTimeHistoricalTimeResolver());

        var result = useCase.Execute(
            new DateOnly(2025, 3, 30),
            new TimeOnly(2, 30),
            "Europe/Madrid");

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Skipped,
            result.Resolution.Status);
    }
}
