using System.Collections.ObjectModel;
using Miastro.Application.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
    public ObservableCollection<PersonHistoryRowViewModel>
        PersonHistory { get; } = [];

    public bool HasPersonHistory
        => PersonHistory.Count > 0;

    public void LoadPersonHistory(
        IReadOnlyList<PersonHistoryReadModel> history)
    {
        PersonHistory.Clear();

        foreach (var item in history
            .OrderByDescending(x => x.OccurredAtUtc))
        {
            PersonHistory.Add(
                new PersonHistoryRowViewModel(
                    item.Summary,
                    item.OccurredAtUtc));
        }

        OnPropertyChanged(
            nameof(HasPersonHistory));
    }

    public void ResetPersonHistory()
    {
        PersonHistory.Clear();

        OnPropertyChanged(
            nameof(HasPersonHistory));
    }
}
