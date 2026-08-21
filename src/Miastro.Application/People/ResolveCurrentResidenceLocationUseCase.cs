using Miastro.Application.Geography;

namespace Miastro.Application.People;

public sealed class ResolveCurrentResidenceLocationUseCase(ILocationSearchService service)
{
    public Task<IReadOnlyList<LocationSearchResult>> ExecuteAsync(
        string text,
        int limit = 25,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult<IReadOnlyList<LocationSearchResult>>(
                Array.Empty<LocationSearchResult>());
        }

        return service.SearchAsync(
            new LocationSearchQuery(text.Trim(), limit),
            cancellationToken);
    }
}
