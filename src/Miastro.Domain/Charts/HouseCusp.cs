using Miastro.Domain.Angles;
using Miastro.Domain.Houses;

namespace Miastro.Domain.Charts;

public readonly record struct HouseCusp(
    AstrologicalHouse House,
    EclipticLongitude Longitude);
