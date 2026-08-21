using Miastro.Application.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record PersonSortChoice(
    string Label,
    PersonSort Value)
{
    public override string ToString()
        => Label;
}
