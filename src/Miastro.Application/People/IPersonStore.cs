namespace Miastro.Application.People;

public interface IPersonStore
{
    Task<Guid> CreateAsync(
        CreatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        UpdatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<PersonDetails?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonListItem>> SearchAsync(
        PersonSearchQuery query,
        CancellationToken cancellationToken = default);

    Task SetFavoriteAsync(
        Guid id,
        bool isFavorite,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task RecordConsultationAsync(
        Guid id,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
