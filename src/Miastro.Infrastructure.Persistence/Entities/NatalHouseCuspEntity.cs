namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class NatalHouseCuspEntity
{
    public Guid ChartId { get; set; }
    public int HouseNumber { get; set; }
    public double LongitudeDegrees { get; set; }

    public NatalChartEntity Chart { get; set; } = null!;
}
