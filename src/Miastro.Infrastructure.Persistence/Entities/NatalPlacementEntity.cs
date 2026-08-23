namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class NatalPlacementEntity
{
    public Guid ChartId { get; set; }
    public int ObjectId { get; set; }

    public double LongitudeDegrees { get; set; }
    public double? LatitudeDegrees { get; set; }
    public double? DistanceAu { get; set; }

    public double? LongitudeSpeedDegreesPerDay { get; set; }
    public double? LatitudeSpeedDegreesPerDay { get; set; }
    public double? DistanceSpeedAuPerDay { get; set; }

    public int? Motion { get; set; }
    public int ZodiacSign { get; set; }
    public double DegreeInSign { get; set; }
    public int? HouseNumber { get; set; }

    public NatalChartEntity Chart { get; set; } = null!;
}
