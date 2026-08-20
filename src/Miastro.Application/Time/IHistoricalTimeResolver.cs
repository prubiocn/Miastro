using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Application.Time;

public interface IHistoricalTimeResolver
{
    HistoricalTimeResolution Resolve(
        LocalDateTime localDateTime,
        IanaTimeZoneId timeZoneId);
}
