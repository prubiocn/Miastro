using Miastro.Domain.Zodiac;

namespace Miastro.Application.Natal.Reading;

public static class NatalDistributionSignCatalog
{
    public static NatalDistributionElement Element(
        ZodiacSign sign)
        => sign switch
        {
            ZodiacSign.Aries
                or ZodiacSign.Leo
                or ZodiacSign.Sagittarius
                => NatalDistributionElement.Fire,

            ZodiacSign.Taurus
                or ZodiacSign.Virgo
                or ZodiacSign.Capricorn
                => NatalDistributionElement.Earth,

            ZodiacSign.Gemini
                or ZodiacSign.Libra
                or ZodiacSign.Aquarius
                => NatalDistributionElement.Air,

            ZodiacSign.Cancer
                or ZodiacSign.Scorpio
                or ZodiacSign.Pisces
                => NatalDistributionElement.Water,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(sign))
        };

    public static NatalDistributionModality Modality(
        ZodiacSign sign)
        => sign switch
        {
            ZodiacSign.Aries
                or ZodiacSign.Cancer
                or ZodiacSign.Libra
                or ZodiacSign.Capricorn
                => NatalDistributionModality.Cardinal,

            ZodiacSign.Taurus
                or ZodiacSign.Leo
                or ZodiacSign.Scorpio
                or ZodiacSign.Aquarius
                => NatalDistributionModality.Fixed,

            ZodiacSign.Gemini
                or ZodiacSign.Virgo
                or ZodiacSign.Sagittarius
                or ZodiacSign.Pisces
                => NatalDistributionModality.Mutable,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(sign))
        };

    public static NatalDistributionPolarity Polarity(
        ZodiacSign sign)
        => sign switch
        {
            ZodiacSign.Aries
                or ZodiacSign.Gemini
                or ZodiacSign.Leo
                or ZodiacSign.Libra
                or ZodiacSign.Sagittarius
                or ZodiacSign.Aquarius
                => NatalDistributionPolarity.Positive,

            ZodiacSign.Taurus
                or ZodiacSign.Cancer
                or ZodiacSign.Virgo
                or ZodiacSign.Scorpio
                or ZodiacSign.Capricorn
                or ZodiacSign.Pisces
                => NatalDistributionPolarity.Negative,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(sign))
        };

    public static string ElementLabel(
        NatalDistributionElement value)
        => value switch
        {
            NatalDistributionElement.Fire =>
                "Fuego",

            NatalDistributionElement.Earth =>
                "Tierra",

            NatalDistributionElement.Air =>
                "Aire",

            NatalDistributionElement.Water =>
                "Agua",

            _ =>
                value.ToString()
        };

    public static string ModalityLabel(
        NatalDistributionModality value)
        => value switch
        {
            NatalDistributionModality.Cardinal =>
                "Cardinal",

            NatalDistributionModality.Fixed =>
                "Fijo",

            NatalDistributionModality.Mutable =>
                "Mutable",

            _ =>
                value.ToString()
        };

    public static string PolarityLabel(
        NatalDistributionPolarity value)
        => value switch
        {
            NatalDistributionPolarity.Positive =>
                "Positiva",

            NatalDistributionPolarity.Negative =>
                "Negativa",

            _ =>
                value.ToString()
        };
}
