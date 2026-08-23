namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class NatalAspectEntity
{
    public Guid ChartId { get; set; }
    public int FirstObject { get; set; }
    public int SecondObject { get; set; }

    public int Kind { get; set; }

    public double SeparationDegrees { get; set; }
    public double ExactAngleDegrees { get; set; }
    public double DeviationDegrees { get; set; }
    public double AllowedOrbDegrees { get; set; }
    public double UsedOrbDegrees { get; set; }

    public NatalChartEntity Chart { get; set; } = null!;
}
