namespace Miastro.Domain.Objects;

public static class AstrologicalObjectCatalog
{
    public static AstrologicalObjectCategory GetCategory(
        AstrologicalObjectId id) =>
        id switch
        {
            AstrologicalObjectId.Sun or
            AstrologicalObjectId.Moon =>
                AstrologicalObjectCategory.Luminary,

            AstrologicalObjectId.Mercury or
            AstrologicalObjectId.Venus or
            AstrologicalObjectId.Mars or
            AstrologicalObjectId.Jupiter or
            AstrologicalObjectId.Saturn or
            AstrologicalObjectId.Uranus or
            AstrologicalObjectId.Neptune or
            AstrologicalObjectId.Pluto =>
                AstrologicalObjectCategory.Planet,

            AstrologicalObjectId.Chiron or
            AstrologicalObjectId.Ceres or
            AstrologicalObjectId.Pallas or
            AstrologicalObjectId.Juno or
            AstrologicalObjectId.Vesta =>
                AstrologicalObjectCategory.MinorBody,

            AstrologicalObjectId.NorthTrueNode or
            AstrologicalObjectId.SouthNode =>
                AstrologicalObjectCategory.Node,

            AstrologicalObjectId.MeanLilith or
            AstrologicalObjectId.PartOfFortune =>
                AstrologicalObjectCategory.CalculatedPoint,

            AstrologicalObjectId.Ascendant or
            AstrologicalObjectId.Midheaven =>
                AstrologicalObjectCategory.Angle,

            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };
}
