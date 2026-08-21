namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class PersonHistoryEntity
{
    public long Id { get; set; }
    public Guid PersonId { get; set; }
    public PersonEntity Person { get; set; } = null!;

    public int EventType { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string Summary { get; set; } = string.Empty;
}
