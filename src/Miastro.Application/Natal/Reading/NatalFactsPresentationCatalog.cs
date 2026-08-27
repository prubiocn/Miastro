using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Application.Natal.Reading;

public static class NatalFactsPresentationCatalog
{
    public static string ObjectName(
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

    public static string ObjectGlyphText(
        AstrologicalObjectId objectId)
        => objectId switch
        {
            AstrologicalObjectId.Sun =>
                "☉",

            AstrologicalObjectId.Moon =>
                "☽",

            AstrologicalObjectId.Mercury =>
                "☿",

            AstrologicalObjectId.Venus =>
                "♀",

            AstrologicalObjectId.Mars =>
                "♂",

            AstrologicalObjectId.Jupiter =>
                "♃",

            AstrologicalObjectId.Saturn =>
                "♄",

            AstrologicalObjectId.Uranus =>
                "♅",

            AstrologicalObjectId.Neptune =>
                "♆",

            AstrologicalObjectId.Pluto =>
                "♇",

            AstrologicalObjectId.NorthTrueNode =>
                "☊",

            AstrologicalObjectId.SouthNode =>
                "☋",

            AstrologicalObjectId.MeanLilith =>
                "⚸",

            AstrologicalObjectId.PartOfFortune =>
                "⊗",

            AstrologicalObjectId.Chiron =>
                "⚷",

            AstrologicalObjectId.Ceres =>
                "⚳",

            AstrologicalObjectId.Pallas =>
                "⚴",

            AstrologicalObjectId.Juno =>
                "⚵",

            AstrologicalObjectId.Vesta =>
                "⚶",

            AstrologicalObjectId.Ascendant =>
                "ASC",

            AstrologicalObjectId.Midheaven =>
                "MC",

            _ =>
                "•"
        };

    public static string SignName(
        ZodiacSign sign)
        => sign switch
        {
            ZodiacSign.Aries =>
                "Aries",

            ZodiacSign.Taurus =>
                "Tauro",

            ZodiacSign.Gemini =>
                "Géminis",

            ZodiacSign.Cancer =>
                "Cáncer",

            ZodiacSign.Leo =>
                "Leo",

            ZodiacSign.Virgo =>
                "Virgo",

            ZodiacSign.Libra =>
                "Libra",

            ZodiacSign.Scorpio =>
                "Escorpio",

            ZodiacSign.Sagittarius =>
                "Sagitario",

            ZodiacSign.Capricorn =>
                "Capricornio",

            ZodiacSign.Aquarius =>
                "Acuario",

            ZodiacSign.Pisces =>
                "Piscis",

            _ =>
                sign.ToString()
        };

    public static string RulersText(
        IReadOnlyList<AstrologicalObjectId> rulers)
        => rulers.Count == 0
            ? "—"
            : string.Join(
                " / ",
                rulers.Select(ObjectName));

    public static bool IsAngle(
        AstrologicalObjectId objectId)
        => objectId
            is AstrologicalObjectId.Ascendant
            or AstrologicalObjectId.Midheaven;
}
