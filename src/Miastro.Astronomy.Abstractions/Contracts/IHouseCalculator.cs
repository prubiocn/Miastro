using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Houses;

namespace Miastro.Astronomy.Abstractions.Contracts;

public interface IHouseCalculator
{
    HouseCalculationResult Calculate(
        AstronomicalInstant instant,
        GeographicLocation location,
        HouseSystem houseSystem);
}
