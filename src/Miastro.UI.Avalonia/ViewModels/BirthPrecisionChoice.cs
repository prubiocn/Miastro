using Miastro.Domain.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record BirthPrecisionChoice(
    string Label,
    BirthTimePrecision Value)
{
    public override string ToString() => Label;
}
