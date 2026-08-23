using Miastro.Domain.Houses;

namespace Miastro.Application.Natal;

public sealed class RecalculateNatalChartUseCase(
    CalculateNatalChartUseCase calculateNatalChart)
{
    public Task<NatalCalculationResult> ExecuteAsync(
        Guid personId,
        HouseSystem houseSystem = HouseSystem.Placidus,
        DateTimeOffset? calculatedAtUtc = null,
        CancellationToken cancellationToken = default)
        => calculateNatalChart.ExecuteAsync(
            personId,
            houseSystem,
            calculatedAtUtc,
            cancellationToken);
}
