using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;

namespace Miastro.Astronomy.Abstractions.Contracts;

public interface IEclipticPositionCalculator
{
    EclipticPosition Calculate(
        AstrologicalObjectId objectId,
        AstronomicalInstant instant,
        CalculationProfile profile);
}
