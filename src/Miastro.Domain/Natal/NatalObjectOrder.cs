using Miastro.Domain.Objects;

namespace Miastro.Domain.Natal;

public static class NatalObjectOrder
{
    public static IReadOnlyList<AstrologicalObjectId> All { get; } =
    [
        AstrologicalObjectId.Sun,
        AstrologicalObjectId.Moon,
        AstrologicalObjectId.Mercury,
        AstrologicalObjectId.Venus,
        AstrologicalObjectId.Mars,
        AstrologicalObjectId.Jupiter,
        AstrologicalObjectId.Saturn,
        AstrologicalObjectId.Uranus,
        AstrologicalObjectId.Neptune,
        AstrologicalObjectId.Pluto,
        AstrologicalObjectId.NorthTrueNode,
        AstrologicalObjectId.SouthNode,
        AstrologicalObjectId.MeanLilith,
        AstrologicalObjectId.PartOfFortune,
        AstrologicalObjectId.Chiron,
        AstrologicalObjectId.Ceres,
        AstrologicalObjectId.Pallas,
        AstrologicalObjectId.Juno,
        AstrologicalObjectId.Vesta,
        AstrologicalObjectId.Ascendant,
        AstrologicalObjectId.Midheaven
    ];

    public static int GetIndex(
        AstrologicalObjectId objectId)
    {
        for (var i = 0; i < All.Count; i++)
        {
            if (All[i] == objectId)
            {
                return i;
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(objectId));
    }
}
