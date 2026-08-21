using Miastro.Application.Time;
using Miastro.Domain.People;

namespace Miastro.Application.People;

public static class BirthHistoricalTimeSnapshotMapper
{
    public static BirthDataWriteModel Apply(
        BirthDataWriteModel source,
        HistoricalTimeResolution resolution,
        int? selectedCandidate = null,
        DateTimeOffset? selectionRecordedAtUtc = null)
    {
        if (source.TimePrecision
            is not BirthTimePrecision.Exact
            and not BirthTimePrecision.Approximate)
        {
            throw new InvalidOperationException(
                "Historical resolution only applies to Exact or Approximate birth time.");
        }

        return resolution.Status switch
        {
            HistoricalTimeResolutionStatus.Resolved =>
                ApplyResolved(
                    source,
                    resolution),

            HistoricalTimeResolutionStatus.Ambiguous =>
                ApplyAmbiguous(
                    source,
                    resolution,
                    selectedCandidate,
                    selectionRecordedAtUtc),

            HistoricalTimeResolutionStatus.Skipped =>
                ApplySkipped(
                    source,
                    resolution),

            _ =>
                throw new InvalidOperationException(
                    "Unsupported historical time resolution state.")
        };
    }

    public static BirthDataWriteModel Invalidate(
        BirthDataWriteModel source)
        => source with
        {
            TzdbVersion = null,
            ResolutionState =
                source.TimePrecision
                    is BirthTimePrecision.Exact
                    or BirthTimePrecision.Approximate
                ? BirthTemporalResolutionState.Pending
                : BirthTemporalResolutionState.NotApplicable,
            HistoricalOffsetSeconds = null,
            ResolvedInstantUtc = null,
            AmbiguousEarlierOffsetSeconds = null,
            AmbiguousEarlierInstantUtc = null,
            AmbiguousLaterOffsetSeconds = null,
            AmbiguousLaterInstantUtc = null,
            AmbiguousSelectedCandidate = null,
            AmbiguousSelectionRecordedAtUtc = null
        };

    private static BirthDataWriteModel ApplyResolved(
        BirthDataWriteModel source,
        HistoricalTimeResolution resolution)
    {
        var candidate =
            resolution.SingleCandidate
            ?? throw new InvalidOperationException(
                "Resolved time must contain one candidate.");

        return source with
        {
            TzdbVersion = resolution.TzdbVersion,
            ResolutionState =
                BirthTemporalResolutionState.Resolved,
            HistoricalOffsetSeconds =
                checked(
                    (int)candidate.Offset
                        .ToTimeSpan()
                        .TotalSeconds),
            ResolvedInstantUtc =
                candidate.Instant.ToDateTimeOffset(),
            AmbiguousEarlierOffsetSeconds = null,
            AmbiguousEarlierInstantUtc = null,
            AmbiguousLaterOffsetSeconds = null,
            AmbiguousLaterInstantUtc = null,
            AmbiguousSelectedCandidate = null,
            AmbiguousSelectionRecordedAtUtc = null
        };
    }

    private static BirthDataWriteModel ApplyAmbiguous(
        BirthDataWriteModel source,
        HistoricalTimeResolution resolution,
        int? selectedCandidate,
        DateTimeOffset? selectionRecordedAtUtc)
    {
        if (resolution.Candidates.Count != 2)
        {
            throw new InvalidOperationException(
                "Ambiguous time must expose two candidates.");
        }

        if (selectedCandidate is not 1 and not 2)
        {
            throw new InvalidOperationException(
                "Ambiguous birth time requires explicit candidate selection.");
        }

        if (selectionRecordedAtUtc is null)
        {
            throw new InvalidOperationException(
                "Ambiguous choice requires an audit timestamp.");
        }

        var first = resolution.Candidates[0];
        var second = resolution.Candidates[1];

        var selected =
            selectedCandidate == 1
                ? first
                : second;

        return source with
        {
            TzdbVersion = resolution.TzdbVersion,
            ResolutionState =
                BirthTemporalResolutionState.Ambiguous,

            HistoricalOffsetSeconds =
                checked(
                    (int)selected.Offset
                        .ToTimeSpan()
                        .TotalSeconds),

            ResolvedInstantUtc =
                selected.Instant.ToDateTimeOffset(),

            AmbiguousEarlierOffsetSeconds =
                checked(
                    (int)first.Offset
                        .ToTimeSpan()
                        .TotalSeconds),

            AmbiguousEarlierInstantUtc =
                first.Instant.ToDateTimeOffset(),

            AmbiguousLaterOffsetSeconds =
                checked(
                    (int)second.Offset
                        .ToTimeSpan()
                        .TotalSeconds),

            AmbiguousLaterInstantUtc =
                second.Instant.ToDateTimeOffset(),

            AmbiguousSelectedCandidate =
                selectedCandidate,

            AmbiguousSelectionRecordedAtUtc =
                selectionRecordedAtUtc.Value
                    .ToUniversalTime()
        };
    }

    private static BirthDataWriteModel ApplySkipped(
        BirthDataWriteModel source,
        HistoricalTimeResolution resolution)
        => source with
        {
            TzdbVersion = resolution.TzdbVersion,
            ResolutionState =
                BirthTemporalResolutionState.Skipped,
            HistoricalOffsetSeconds = null,
            ResolvedInstantUtc = null,
            AmbiguousEarlierOffsetSeconds = null,
            AmbiguousEarlierInstantUtc = null,
            AmbiguousLaterOffsetSeconds = null,
            AmbiguousLaterInstantUtc = null,
            AmbiguousSelectedCandidate = null,
            AmbiguousSelectionRecordedAtUtc = null
        };
}
