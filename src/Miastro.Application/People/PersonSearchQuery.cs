namespace Miastro.Application.People;

public sealed record PersonSearchQuery(
    string? Text,
    PersonFilter Filter = PersonFilter.All,
    PersonSort Sort = PersonSort.FirstName,
    int Limit = 100);
