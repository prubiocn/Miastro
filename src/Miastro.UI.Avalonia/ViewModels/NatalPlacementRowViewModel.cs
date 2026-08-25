using Miastro.Application.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record NatalPlacementRowViewModel(
    string ObjectName,
    string PositionText,
    string HouseText,
    string MotionText)
{
    public AstrologicalObjectId ObjectId
        { get; init; }

    public static NatalPlacementRowViewModel From(
        NatalPlacementSnapshot placement)
        => new(
            ObjectLabel(
                placement.ObjectId),
            FormatPosition(
                placement.LongitudeDegrees),
            placement.HouseNumber is int house
                ? $"Casa {house}"
                : "—",
            MotionLabel(
                placement.Motion))
        {
            ObjectId =
                placement.ObjectId
        };

    private static string FormatPosition(
        double longitude)
    {
        var normalized =
            ((longitude % 360.0) + 360.0)
            % 360.0;

        var signIndex =
            (int)Math.Floor(
                normalized / 30.0);

        var degreeInSign =
            normalized
            - signIndex * 30.0;

        var degrees =
            (int)Math.Floor(
                degreeInSign);

        var minutes =
            (int)Math.Round(
                (degreeInSign - degrees)
                * 60.0);

        if (minutes == 60)
        {
            minutes = 0;
            degrees++;

            if (degrees == 30)
            {
                degrees = 0;
                signIndex =
                    (signIndex + 1) % 12;
            }
        }

        return
            $"{degrees:00}° {minutes:00}′ "
            + SignNames[signIndex];
    }

    private static string MotionLabel(
        MotionState? motion)
        => motion switch
        {
            MotionState.Direct =>
                "Directo",

            MotionState.Retrograde =>
                "Retrógrado",

            MotionState.Stationary =>
                "Estacionario",

            _ =>
                "—"
        };

    internal static string ObjectLabel(
        AstrologicalObjectId objectId)
        => objectId switch
        {
            AstrologicalObjectId.Sun =>
                "Sol",

            AstrologicalObjectId.Moon =>
                "Luna",

            AstrologicalObjectId.Mercury =>
                "Mercurio",

            AstrologicalObjectId.Venus =>
                "Venus",

            AstrologicalObjectId.Mars =>
                "Marte",

            AstrologicalObjectId.Jupiter =>
                "Júpiter",

            AstrologicalObjectId.Saturn =>
                "Saturno",

            AstrologicalObjectId.Uranus =>
                "Urano",

            AstrologicalObjectId.Neptune =>
                "Neptuno",

            AstrologicalObjectId.Pluto =>
                "Plutón",

            AstrologicalObjectId.NorthTrueNode =>
                "Nodo Norte verdadero",

            AstrologicalObjectId.SouthNode =>
                "Nodo Sur",

            AstrologicalObjectId.MeanLilith =>
                "Lilith media",

            AstrologicalObjectId.PartOfFortune =>
                "Parte de Fortuna",

            AstrologicalObjectId.Chiron =>
                "Quirón",

            AstrologicalObjectId.Ceres =>
                "Ceres",

            AstrologicalObjectId.Pallas =>
                "Palas",

            AstrologicalObjectId.Juno =>
                "Juno",

            AstrologicalObjectId.Vesta =>
                "Vesta",

            AstrologicalObjectId.Ascendant =>
                "Ascendente",

            AstrologicalObjectId.Midheaven =>
                "Medio Cielo",

            _ =>
                objectId.ToString()
        };

    private static readonly string[]
        SignNames =
    [
        "Aries",
        "Tauro",
        "Géminis",
        "Cáncer",
        "Leo",
        "Virgo",
        "Libra",
        "Escorpio",
        "Sagitario",
        "Capricornio",
        "Acuario",
        "Piscis"
    ];
}
