namespace Miastro.Application.People;

public sealed class UpdateResidenceUseCase(
    GetPersonUseCase getPerson,
    UpdatePersonUseCase updatePerson)
{
    public async Task ExecuteAsync(
        Guid personId,
        CurrentResidenceWriteModel? residence,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (personId == Guid.Empty)
        {
            throw new ArgumentException(
                "Person id is required.",
                nameof(personId));
        }

        var person =
            await getPerson.ExecuteAsync(
                personId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                "Person was not found.");

        await updatePerson.ExecuteAsync(
            new UpdatePersonCommand(
                person.Id,
                person.FirstName,
                person.LastName,
                person.Phone,
                person.Email,
                person.PrivateNote,
                person.IsFavorite,
                person.BirthData is null
                    ? null
                    : PersonWriteModelMapper.ToWriteModel(
                        person.BirthData),
                residence),
            nowUtc,
            cancellationToken);
    }
}
