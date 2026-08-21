namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class PersonEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PrivateNote { get; set; }

    public bool IsFavorite { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ModifiedAtUtc { get; set; }
    public DateTimeOffset? LastConsultationAtUtc { get; set; }

    public BirthDataEntity? BirthData { get; set; }
    public CurrentResidenceEntity? CurrentResidence { get; set; }
    public List<PersonHistoryEntity> History { get; set; } = [];
}
