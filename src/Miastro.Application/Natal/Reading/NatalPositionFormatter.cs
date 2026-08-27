using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Application.Natal.Reading;

public static class NatalPositionFormatter
{
    public static string ExactPosition(
        double longitudeDegrees)
    {
        var normalized =
            Normalize(
                longitudeDegrees);

        var signIndex =
            (int)Math.Floor(
                normalized / 30.0);

        var degreeInSign =
            normalized
            - signIndex * 30.0;

        var totalMinutes =
            (int)Math.Round(
                degreeInSign * 60.0,
                MidpointRounding.AwayFromZero);

        if (totalMinutes >= 30 * 60)
        {
            totalMinutes = 0;
            signIndex =
                (signIndex + 1) % 12;
        }

        var degrees =
            totalMinutes / 60;

        var minutes =
            totalMinutes % 60;

        var sign =
            (ZodiacSign)signIndex;

        return
            $"{degrees:00}° {minutes:00}′ "
            + NatalFactsPresentationCatalog
                .SignName(sign);
    }

    public static string DegreeOnly(
        double longitudeDegrees)
    {
        var normalized =
            Normalize(
                longitudeDegrees);

        var degreeInSign =
            normalized % 30.0;

        var totalMinutes =
            (int)Math.Round(
                degreeInSign * 60.0,
                MidpointRounding.AwayFromZero);

        if (totalMinutes >= 30 * 60)
        {
            totalMinutes = 0;
        }

        return
            $"{totalMinutes / 60:00}° "
            + $"{totalMinutes % 60:00}′";
    }

    public static string House(
        int? houseNumber)
        => houseNumber is int house
            ? $"Casa {house}"
            : "—";

    public static string Motion(
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

    private static double Normalize(
        double longitudeDegrees)
    {
        if (!double.IsFinite(
            longitudeDegrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(longitudeDegrees));
        }

        return
            ((longitudeDegrees % 360.0)
             + 360.0)
            % 360.0;
    }
}
