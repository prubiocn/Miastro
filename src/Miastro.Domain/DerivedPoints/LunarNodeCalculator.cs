using Miastro.Domain.Angles;

namespace Miastro.Domain.DerivedPoints;

public static class LunarNodeCalculator
{
    public static EclipticLongitude CalculateSouthNode(
        EclipticLongitude northTrueNode) =>
        northTrueNode + Angle.FromDegrees(180.0);
}
