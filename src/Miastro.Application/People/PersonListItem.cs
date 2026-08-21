namespace Miastro.Application.People;

public sealed record PersonListItem(
    Guid Id,
    string FirstName,
    string LastName,
    bool IsFavorite,
    DateTimeOffset? LastConsultationAtUtc);
