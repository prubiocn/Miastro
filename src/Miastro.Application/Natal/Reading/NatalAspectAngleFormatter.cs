namespace Miastro.Application.Natal.Reading;

public static class NatalAspectAngleFormatter
{
    public static string DegreesMinutes(
        double degrees)
    {
        if (!double.IsFinite(degrees)
            || degrees < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees));
        }

        var totalMinutes =
            (int)Math.Round(
                degrees * 60.0,
                MidpointRounding.AwayFromZero);

        var wholeDegrees =
            totalMinutes / 60;

        var minutes =
            totalMinutes % 60;

        return
            $"{wholeDegrees}°{minutes:00}′";
    }
}
