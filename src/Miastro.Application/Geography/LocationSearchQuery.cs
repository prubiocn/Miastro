namespace Miastro.Application.Geography;

public sealed record LocationSearchQuery(
    string Text,
    int Limit = 20,
    string? CountryCode = null);
