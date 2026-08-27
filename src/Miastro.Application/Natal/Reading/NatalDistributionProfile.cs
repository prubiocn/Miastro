using Miastro.Domain.Objects;
using Miastro.Domain.Natal;

namespace Miastro.Application.Natal.Reading;

public sealed class NatalDistributionProfile
{
    private readonly HashSet<AstrologicalObjectId>
        _countedObjectSet;

    public string Id { get; }

    public IReadOnlyList<AstrologicalObjectId>
        CountedObjects { get; }

    public static NatalDistributionProfile
        MiastroV1 { get; } =
            new(
                "MiastroV1",
                new[]
                {
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Moon,
                    AstrologicalObjectId.Mercury,
                    AstrologicalObjectId.Venus,
                    AstrologicalObjectId.Mars,
                    AstrologicalObjectId.Jupiter,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Uranus,
                    AstrologicalObjectId.Neptune,
                    AstrologicalObjectId.Pluto
                });

    public NatalDistributionProfile(
        string id,
        IReadOnlyList<AstrologicalObjectId>
            countedObjects)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "El identificador del perfil es obligatorio.",
                nameof(id));
        }

        ArgumentNullException.ThrowIfNull(
            countedObjects);

        var ordered =
            countedObjects
                .Distinct()
                .OrderBy(
                    NatalObjectOrder.GetIndex)
                .ToArray();

        if (ordered.Length == 0)
        {
            throw new ArgumentException(
                "El perfil debe incluir al menos un objeto.",
                nameof(countedObjects));
        }

        Id =
            id;

        CountedObjects =
            ordered;

        _countedObjectSet =
            ordered.ToHashSet();
    }

    public bool Includes(
        AstrologicalObjectId objectId)
        => _countedObjectSet.Contains(
            objectId);
}
