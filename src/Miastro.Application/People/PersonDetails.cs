namespace Miastro.Application.People;

public sealed record PersonDetails(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string? PrivateNote,
    bool IsFavorite,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ModifiedAtUtc,
    DateTimeOffset? LastConsultationAtUtc,
    BirthDataReadModel? BirthData,
    CurrentResidenceReadModel? CurrentResidence,
    IReadOnlyList<PersonHistoryReadModel> History);
