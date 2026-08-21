using Miastro.Application.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record PersonFilterChoice(
    string Label,
    PersonFilter Value)
{
    public override string ToString()
        => Label;
}
