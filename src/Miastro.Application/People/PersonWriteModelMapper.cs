namespace Miastro.Application.People;

public static class PersonWriteModelMapper
{
    public static BirthDataWriteModel ToWriteModel(
        BirthDataReadModel source)
        => new(
            source.LocalDate,
            source.TimePrecision,
            source.LocalTime,
            source.RangeStart,
            source.RangeEnd,
            source.DayPeriod,
            source.GeoNameId,
            source.Locality,
            source.Country,
            source.Region,
            source.Subregion,
            source.Latitude,
            source.Longitude,
            source.IanaTimeZoneId,
            source.TzdbVersion,
            source.ResolutionState,
            source.HistoricalOffsetSeconds,
            source.ResolvedInstantUtc,
            source.AmbiguousEarlierOffsetSeconds,
            source.AmbiguousEarlierInstantUtc,
            source.AmbiguousLaterOffsetSeconds,
            source.AmbiguousLaterInstantUtc,
            source.AmbiguousSelectedCandidate,
            source.AmbiguousSelectionRecordedAtUtc,
            source.ManualCoordinateOverride,
            source.OriginalGeoNamesLatitude,
            source.OriginalGeoNamesLongitude);

    public static CurrentResidenceWriteModel ToWriteModel(
        CurrentResidenceReadModel source)
        => new(
            source.Locality,
            source.GeoNameId,
            source.Region,
            source.Country,
            source.Latitude,
            source.Longitude,
            source.IanaTimeZoneId,
            source.UpdatedAtUtc);
}
