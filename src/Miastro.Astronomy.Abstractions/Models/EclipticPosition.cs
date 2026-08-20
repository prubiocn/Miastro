using Miastro.Domain.Angles;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;

namespace Miastro.Astronomy.Abstractions.Models;

public sealed record EclipticPosition(
    AstrologicalObjectId ObjectId,
    EclipticLongitude Longitude,
    double LatitudeDegrees,
    double DistanceAu,
    double LongitudeSpeedDegreesPerDay,
    double LatitudeSpeedDegreesPerDay,
    double DistanceSpeedAuPerDay,
    AstronomicalInstant Instant,
    ReferenceFrame ReferenceFrame,
    IReadOnlyList<string> AppliedFlags,
    AstronomyEngineMetadata EngineMetadata);
