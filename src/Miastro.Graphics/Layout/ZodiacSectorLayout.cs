namespace Miastro.Graphics.Layout;

public sealed record ZodiacSectorLayout(
    int SignIndex,
    double StartLongitudeDegrees,
    double CenterLongitudeDegrees,
    double StartScreenAngleDegrees,
    double SweepAngleDegrees);
