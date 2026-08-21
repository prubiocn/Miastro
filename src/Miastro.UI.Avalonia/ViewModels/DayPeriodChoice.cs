using Miastro.Domain.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record DayPeriodChoice(
    string Label,
    DayPeriod Value)
{
    public override string ToString() => Label;
}
