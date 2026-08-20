namespace Miastro.Application.Geography;

public enum GeographyErrorCode
{
    CatalogMissing,
    CatalogCorrupt,
    SchemaMismatch,
    UnexpectedCatalogVersion,
    InvalidSearch,
    LocationNotFound,
    InvalidTimeZoneId,
    IncompleteResult
}
