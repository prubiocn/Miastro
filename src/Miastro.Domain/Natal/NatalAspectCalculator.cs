using Miastro.Domain.Aspects;
using Miastro.Domain.Placements;

namespace Miastro.Domain.Natal;

public static class NatalAspectCalculator
{
    public static IReadOnlyList<AspectResult> Calculate(
        IEnumerable<AstrologicalPlacement> placements)
    {
        ArgumentNullException.ThrowIfNull(placements);

        var byObject =
            placements
                .ToDictionary(
                    x => x.ObjectId);

        var participants =
            NatalObjectOrder.All
                .Where(x =>
                    MiastroV1AspectProfile.Instance
                        .IsParticipant(x))
                .Where(byObject.ContainsKey)
                .ToArray();

        var results =
            new List<AspectResult>();

        for (var i = 0; i < participants.Length; i++)
        {
            for (var j = i + 1; j < participants.Length; j++)
            {
                var first =
                    byObject[participants[i]];

                var second =
                    byObject[participants[j]];

                var result =
                    AspectEngine.Detect(
                        first.ObjectId,
                        first.Longitude,
                        second.ObjectId,
                        second.Longitude,
                        MiastroV1AspectProfile.Instance);

                if (result is not null)
                {
                    results.Add(result);
                }
            }
        }

        return results;
    }
}
