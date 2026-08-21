using Miastro.Application.Time;
using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Application.People;

public sealed class ResolveBirthHistoricalTimeUseCase(
    IHistoricalTimeResolver resolver)
{
    public BirthHistoricalTimeResolutionResult Execute(
        DateOnly date,
        TimeOnly time,
        string ianaTimeZoneId)
    {
        var local = new LocalDateTime(
            date.Year,
            date.Month,
            date.Day,
            time.Hour,
            time.Minute,
            time.Second,
            time.Millisecond);

        var result = resolver.Resolve(
            local,
            new IanaTimeZoneId(ianaTimeZoneId));

        var message = result.Status switch
        {
            HistoricalTimeResolutionStatus.Resolved =>
                "Zona horaria histórica resuelta correctamente.",
            HistoricalTimeResolutionStatus.Ambiguous =>
                "Hora ambigua: existen dos posibilidades.",
            HistoricalTimeResolutionStatus.Skipped =>
                "Hora inexistente: esa hora local no existió por un cambio horario.",
            _ =>
                "Estado temporal histórico no reconocido."
        };

        return new(result, message);
    }
}
