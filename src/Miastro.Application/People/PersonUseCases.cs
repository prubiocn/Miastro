namespace Miastro.Application.People;

public sealed class CreatePersonUseCase(
    IPersonStore store)
{
    public Task<Guid> ExecuteAsync(
        CreatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        PersonInputValidator.Validate(command);

        return store.CreateAsync(
            command,
            nowUtc.ToUniversalTime(),
            cancellationToken);
    }
}

public sealed class UpdatePersonUseCase(
    IPersonStore store)
{
    public Task ExecuteAsync(
        UpdatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        PersonInputValidator.Validate(command);

        return store.UpdateAsync(
            command,
            nowUtc.ToUniversalTime(),
            cancellationToken);
    }
}

public sealed class GetPersonUseCase(
    IPersonStore store)
{
    public Task<PersonDetails?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => store.GetAsync(id, cancellationToken);
}

public sealed class SearchPeopleUseCase(
    IPersonStore store)
{
    public Task<IReadOnlyList<PersonListItem>> ExecuteAsync(
        PersonSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.Limit is < 1 or > 500)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query),
                "Limit must be between 1 and 500.");
        }

        return store.SearchAsync(
            query,
            cancellationToken);
    }
}

public sealed class DeletePersonUseCase(
    IPersonStore store)
{
    public Task ExecuteAsync(
        Guid id,
        bool confirmed,
        CancellationToken cancellationToken = default)
    {
        if (!confirmed)
        {
            throw new InvalidOperationException(
                "Explicit deletion confirmation is required.");
        }

        return store.DeleteAsync(
            id,
            cancellationToken);
    }
}

public sealed class SetFavoriteUseCase(
    IPersonStore store)
{
    public Task ExecuteAsync(
        Guid id,
        bool isFavorite,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
        => store.SetFavoriteAsync(
            id,
            isFavorite,
            nowUtc.ToUniversalTime(),
            cancellationToken);
}

public sealed class RecordPersonConsultationUseCase(
    IPersonStore store)
{
    public Task ExecuteAsync(
        Guid id,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
        => store.RecordConsultationAsync(
            id,
            nowUtc.ToUniversalTime(),
            cancellationToken);
}
