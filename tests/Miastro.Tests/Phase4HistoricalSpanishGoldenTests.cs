using Miastro.Application.Time;
using Miastro.Domain.Geography;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;
using NodaTime.Text;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4HistoricalSpanishGoldenTests
{
    private static readonly LocalDateTimePattern LocalPattern =
        LocalDateTimePattern.CreateWithInvariantCulture(
            "yyyy-MM-dd'T'HH:mm:ss");

    private static readonly InstantPattern InstantPattern =
        InstantPattern.General;

    private static readonly OffsetPattern OffsetPattern =
        OffsetPattern.GeneralInvariant;

    [TestMethod]
    public void HistoricalCorpus_MatchesBundledTzdb()
    {
        var resolver = new NodaTimeHistoricalTimeResolver();

        foreach (var row in ReadRows())
        {
            var local = LocalPattern.Parse(row.Local);

            Assert.IsTrue(
                local.Success,
                row.Id);

            var result = resolver.Resolve(
                local.Value,
                new IanaTimeZoneId(row.Zone));

            Assert.AreEqual(
                row.Status,
                result.Status.ToString(),
                row.Id);

            switch (result.Status)
            {
                case HistoricalTimeResolutionStatus.Resolved:
                    AssertResolved(row, result);
                    break;

                case HistoricalTimeResolutionStatus.Ambiguous:
                    AssertAmbiguous(row, result);
                    break;

                case HistoricalTimeResolutionStatus.Skipped:
                    Assert.AreEqual(
                        0,
                        result.Candidates.Count,
                        row.Id);

                    Assert.IsNotNull(
                        result.Transition,
                        row.Id);

                    break;

                default:
                    Assert.Fail(
                        $"Unsupported status in golden: {row.Id}");
                    break;
            }
        }
    }

    private static void AssertResolved(
        GoldenRow row,
        HistoricalTimeResolution result)
    {
        Assert.AreEqual(
            1,
            result.Candidates.Count,
            row.Id);

        Assert.AreEqual(
            ParseOffset(row.Offset1, row.Id),
            result.Candidates[0].Offset,
            row.Id);

        Assert.AreEqual(
            ParseInstant(row.Instant1, row.Id),
            result.Candidates[0].Instant,
            row.Id);
    }

    private static void AssertAmbiguous(
        GoldenRow row,
        HistoricalTimeResolution result)
    {
        Assert.AreEqual(
            2,
            result.Candidates.Count,
            row.Id);

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(row.Offset2),
            row.Id);

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(row.Instant2),
            row.Id);

        var expected = new[]
        {
            (
                Offset: ParseOffset(row.Offset1, row.Id),
                Instant: ParseInstant(row.Instant1, row.Id)
            ),
            (
                Offset: ParseOffset(row.Offset2!, row.Id),
                Instant: ParseInstant(row.Instant2!, row.Id)
            )
        }
        .OrderBy(x => x.Instant)
        .ToArray();

        var actual = result.Candidates
            .Select(
                x => (
                    Offset: x.Offset,
                    Instant: x.Instant))
            .OrderBy(x => x.Instant)
            .ToArray();

        CollectionAssert.AreEqual(
            expected,
            actual,
            row.Id);
    }

    private static Offset ParseOffset(
        string value,
        string id)
    {
        var parsed = OffsetPattern.Parse(value);

        Assert.IsTrue(
            parsed.Success,
            $"{id}: invalid offset {value}");

        return parsed.Value;
    }

    private static Instant ParseInstant(
        string value,
        string id)
    {
        var parsed = InstantPattern.Parse(value);

        Assert.IsTrue(
            parsed.Success,
            $"{id}: invalid instant {value}");

        return parsed.Value;
    }

    private static IEnumerable<GoldenRow> ReadRows()
    {
        var path = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/time/goldens/"
                + "historical-time-spanish.tsv"));

        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) ||
                line.StartsWith('#'))
            {
                continue;
            }

            var p = line.Split('\t');

            yield return new GoldenRow(
                p[0],
                p[1],
                p[2],
                p[3],
                p[4],
                p[5],
                p.Length > 6 ? p[6] : null,
                p.Length > 7 ? p[7] : null,
                p.Length > 8 ? p[8] : null);
        }
    }

    private sealed record GoldenRow(
        string Id,
        string Zone,
        string Local,
        string Status,
        string Offset1,
        string Instant1,
        string? Offset2,
        string? Instant2,
        string? Source);
}
