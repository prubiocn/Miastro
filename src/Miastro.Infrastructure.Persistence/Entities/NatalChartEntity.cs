namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class NatalChartEntity
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }

    public int Status { get; set; }
    public string InputHash { get; set; } = string.Empty;

    public int BirthDataVersion { get; set; } = 1;

    public string BirthDataHash { get; set; } =
        string.Empty;

    public int BirthTimePrecision { get; set; }

    public long GeoNameId { get; set; }

    public int? HistoricalOffsetSeconds { get; set; }

    public string? AmbiguousSelection { get; set; }

    public bool IsApproximateBirthTime { get; set; }

    public DateOnly BirthLocalDate { get; set; }
    public TimeOnly BirthLocalTime { get; set; }
    public DateTimeOffset InstantUtc { get; set; }

    public string Locality { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IanaTimeZoneId { get; set; } = string.Empty;
    public string TzdbVersion { get; set; } = string.Empty;

    public int HouseSystem { get; set; }
    public string CalculationProfileId { get; set; } = string.Empty;

    public string MiastroVersion { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public string EngineVersion { get; set; } = string.Empty;
    public string AdapterVersion { get; set; } = string.Empty;
    public string EphemerisVersion { get; set; } = string.Empty;

    public DateTimeOffset CalculatedAtUtc { get; set; }
    public DateTimeOffset? InvalidatedAtUtc { get; set; }
    public Guid? SupersededByChartId { get; set; }

    public List<NatalPlacementEntity> Placements { get; set; } = [];
    public List<NatalHouseCuspEntity> HouseCusps { get; set; } = [];
    public List<NatalAspectEntity> Aspects { get; set; } = [];
}
