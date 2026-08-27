using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public enum NatalDistributionElement
{
    Fire = 0,
    Earth = 1,
    Air = 2,
    Water = 3
}

public enum NatalDistributionModality
{
    Cardinal = 0,
    Fixed = 1,
    Mutable = 2
}

public enum NatalDistributionPolarity
{
    Positive = 0,
    Negative = 1
}

public sealed record NatalDistributionBucket<TCategory>(
    TCategory Category,
    string Label,
    int Count,
    IReadOnlyList<AstrologicalObjectId> Objects,
    IReadOnlyList<string> ObjectNames)
    where TCategory : struct, Enum;

public sealed record NatalDistributionSection<TCategory>(
    IReadOnlyList<NatalDistributionBucket<TCategory>> Buckets,
    TCategory? Predominant,
    bool IsBalanced)
    where TCategory : struct, Enum;

public sealed record NatalDistributionReadModel(
    string ProfileId,
    IReadOnlyList<AstrologicalObjectId> CountedObjects,
    NatalDistributionSection<NatalDistributionElement> Elements,
    NatalDistributionSection<NatalDistributionModality> Modalities,
    NatalDistributionSection<NatalDistributionPolarity> Polarities);
