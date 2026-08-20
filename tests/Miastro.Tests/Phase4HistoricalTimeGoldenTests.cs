using Miastro.Application.Time;
using Miastro.Domain.Geography;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;
using NodaTime.Text;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4HistoricalTimeGoldenTests
{
    private static readonly LocalDateTimePattern LocalPattern =
        LocalDateTimePattern.CreateWithInvariantCulture(
            "yyyy-MM-dd'T'HH:mm:ss");

    private static readonly InstantPattern InstantPattern =
        InstantPattern.General;

    private static readonly OffsetPattern OffsetPattern =
        OffsetPattern.GeneralInvariant;

    [TestMethod]
    public void GoldenCorpus_MatchesBundledTzdb()
    {
        var resolver = new NodaTimeHistoricalTimeResolver();

        foreach (var row in ReadRows())
        {
            var localParse = LocalPattern.Parse(row.Local);

            Assert.IsTrue(
                localParse.Success,
                $"Invalid local golden: {row.Id}");

            var result = resolver.Resolve(
                localParse.Value,
                new IanaTimeZoneId(row.Zone));

            Assert.AreEqual(
                row.Status,
                result.Status.ToString(),
                row.Id);

            if (result.Status ==
                HistoricalTimeResolutionStatus.Resolved)
            {
                Assert.AreEqual(1, result.Candidates.Count);

                Assert.AreEqual(
                    ParseOffset(row.Offset1),
                    result.Candidates[0].Offset,
                    row.Id);

                Assert.AreEqual(
                    ParseInstant(row.Instant1),
                    result.Candidates[0].Instant,
                    row.Id);
            }

            if (result.Status ==
                HistoricalTimeResolutionStatus.Ambiguous)
            {
                Assert.AreEqual(2, result.Candidates.Count);

                var expected = new[]
                {
                    (
                        ParseOffset(row.Offset1),
                        ParseInstant(row.Instant1)
                    ),
                    (
                        ParseOffset(row.Offset2!),
                        ParseInstant(row.Instant2!)
                    )
                };

                var actual = result.Candidates
                    .Select(x => (x.Offset, x.Instant))
                    .OrderBy(x => x.Instant)
                    .ToArray();

                var orderedExpected = expected
                    .OrderBy(x => x.Item2)
                    .ToArray();

                CollectionAssert.AreEqual(
                    orderedExpected,
                    actual,
                    row.Id);
            }

            if (result.Status ==
                HistoricalTimeResolutionStatus.Skipped)
            {
                Assert.AreEqual(0, result.Candidates.Count);
                Assert.IsNotNull(result.Transition);
            }

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(result.TzdbVersion),
                row.Id);
        }
    }

    private static Offset ParseOffset(string value)
    {
        var parsed = OffsetPattern.Parse(value);
        Assert.IsTrue(parsed.Success);
        return parsed.Value;
    }

    private static Instant ParseInstant(string value)
    {
        var parsed = InstantPattern.Parse(value);
        Assert.IsTrue(parsed.Success);
        return parsed.Value;
    }

    private static IEnumerable<GoldenRow> ReadRows()
    {
        var path = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/time/goldens/"
                + "historical-time-goldens.tsv"));

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
                p[6],
                p[7],
                p[8]);
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
        string Source);
}
