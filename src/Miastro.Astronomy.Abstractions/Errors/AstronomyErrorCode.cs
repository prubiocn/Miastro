namespace Miastro.Astronomy.Abstractions.Errors;

public enum AstronomyErrorCode
{
    LibraryNotFound,
    LibraryNotLoadable,
    AbiIncompatible,
    UnexpectedEngineVersion,
    EphemerisFileMissing,
    EphemerisFileCorrupt,
    UnsupportedTimeRange,
    UnsupportedObject,
    CalculationFailed,
    InvalidResult,
    InvalidConfiguration,
    HouseCalculationUnavailable
}
