using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Domain.Objects;

namespace Miastro.Infrastructure.SwissEphemeris.Mapping;

internal static class SwissObjectMapper
{
    public static int ToSwissId(
        AstrologicalObjectId objectId) =>
        objectId switch
        {
            AstrologicalObjectId.Sun =>
                SwissBodyIds.Sun,

            AstrologicalObjectId.Moon =>
                SwissBodyIds.Moon,

            AstrologicalObjectId.Mercury =>
                SwissBodyIds.Mercury,

            AstrologicalObjectId.Venus =>
                SwissBodyIds.Venus,

            AstrologicalObjectId.Mars =>
                SwissBodyIds.Mars,

            AstrologicalObjectId.Jupiter =>
                SwissBodyIds.Jupiter,

            AstrologicalObjectId.Saturn =>
                SwissBodyIds.Saturn,

            AstrologicalObjectId.Uranus =>
                SwissBodyIds.Uranus,

            AstrologicalObjectId.Neptune =>
                SwissBodyIds.Neptune,

            AstrologicalObjectId.Pluto =>
                SwissBodyIds.Pluto,

            AstrologicalObjectId.NorthTrueNode =>
                SwissBodyIds.TrueNode,

            AstrologicalObjectId.MeanLilith =>
                SwissBodyIds.MeanApogee,

            AstrologicalObjectId.Chiron =>
                SwissBodyIds.Chiron,

            AstrologicalObjectId.Ceres =>
                SwissBodyIds.Ceres,

            AstrologicalObjectId.Pallas =>
                SwissBodyIds.Pallas,

            AstrologicalObjectId.Juno =>
                SwissBodyIds.Juno,

            AstrologicalObjectId.Vesta =>
                SwissBodyIds.Vesta,

            _ => throw Unsupported(objectId)
        };

    private static AstronomyEngineException Unsupported(
        AstrologicalObjectId objectId) =>
        new(
            new AstronomyError(
                AstronomyErrorCode.UnsupportedObject,
                "SWISS_OBJECT_UNSUPPORTED",
                "El objeto astronómico solicitado no está soportado por el motor."),
            $"ObjectId={objectId}");
}
