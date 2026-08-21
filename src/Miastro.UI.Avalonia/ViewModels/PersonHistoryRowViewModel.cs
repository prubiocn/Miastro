namespace Miastro.UI.Avalonia.ViewModels;

public sealed record PersonHistoryRowViewModel(
    string Summary,
    DateTimeOffset OccurredAtUtc)
{
    public string WhenText
        => OccurredAtUtc
            .ToLocalTime()
            .ToString("dd/MM/yyyy HH:mm");
}
