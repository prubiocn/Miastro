using Miastro.Application.Time;
using Miastro.Domain.Geography;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4HistoricalTimeTests
{
    private readonly NodaTimeHistoricalTimeResolver _resolver = new();

    [TestMethod]
    public void Madrid_NormalTime_ResolvesExactlyOnce()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 1, 15, 12, 0),
            new IanaTimeZoneId("Europe/Madrid"));

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Resolved,
            result.Status);
        Assert.AreEqual(1, result.Candidates.Count);
        Assert.AreEqual(
            Offset.FromHours(1),
            result.Candidates[0].Offset);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TzdbVersion));
    }

    [TestMethod]
    public void Madrid_AutumnTransition_IsAmbiguous()
    {
        var local = new LocalDateTime(2024, 10, 27, 2, 30);

        var result = _resolver.Resolve(
            local,
            new IanaTimeZoneId("Europe/Madrid"));

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Ambiguous,
            result.Status);
        Assert.AreEqual(2, result.Candidates.Count);
        Assert.AreEqual(local, result.Candidates[0].ZonedDateTime.LocalDateTime);
        Assert.AreEqual(local, result.Candidates[1].ZonedDateTime.LocalDateTime);
        Assert.AreNotEqual(
            result.Candidates[0].Offset,
            result.Candidates[1].Offset);
        Assert.AreNotEqual(
            result.Candidates[0].Instant,
            result.Candidates[1].Instant);
    }

    [TestMethod]
    public void Madrid_SpringTransition_IsSkipped()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 3, 31, 2, 30),
            new IanaTimeZoneId("Europe/Madrid"));

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Skipped,
            result.Status);
        Assert.AreEqual(0, result.Candidates.Count);
        Assert.IsNotNull(result.Transition);
        Assert.AreEqual(
            Offset.FromHours(1),
            result.Transition!.OffsetBefore);
        Assert.AreEqual(
            Offset.FromHours(2),
            result.Transition.OffsetAfter);
    }

    [TestMethod]
    public void Kathmandu_UsesFortyFiveMinuteOffset()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 6, 1, 12, 0),
            new IanaTimeZoneId("Asia/Kathmandu"));

        Assert.AreEqual(
            Offset.FromHoursAndMinutes(5, 45),
            result.SingleCandidate!.Offset);
    }

    [TestMethod]
    public void Adelaide_UsesHalfHourOffset()
    {
        var result = _resolver.Resolve(
            new LocalDateTime(2024, 7, 1, 12, 0),
            new IanaTimeZoneId("Australia/Adelaide"));

        Assert.AreEqual(
            Offset.FromHoursAndMinutes(9, 30),
            result.SingleCandidate!.Offset);
    }

    [TestMethod]
    public void UnknownZone_IsTypedError()
    {
        HistoricalTimeException? captured = null;

        try
        {
            _resolver.Resolve(
                new LocalDateTime(2024, 1, 1, 12, 0),
                new IanaTimeZoneId("Etc/DefinitelyMissing"));
        }
        catch (HistoricalTimeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);

        Assert.AreEqual(
            HistoricalTimeErrorCode.UnknownTimeZone,
            captured.Code);
    }
}
