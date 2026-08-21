namespace Miastro.UI.Avalonia.ViewModels;

public sealed record PersonListRowViewModel(
    Guid Id,
    string FirstName,
    string LastName,
    bool IsFavorite,
    DateTimeOffset? LastConsultationAtUtc)
{
    public string DisplayName
        => string.IsNullOrWhiteSpace(LastName)
            ? FirstName
            : $"{FirstName} {LastName}";

    public string FavoriteMark
        => IsFavorite
            ? "★"
            : string.Empty;

    public string LastConsultationText
        => LastConsultationAtUtc is null
            ? "Sin consultas"
            : $"Última consulta: "
              + LastConsultationAtUtc.Value
                  .ToLocalTime()
                  .ToString("dd/MM/yyyy HH:mm");
}
