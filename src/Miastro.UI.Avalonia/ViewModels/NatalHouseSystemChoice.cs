using Miastro.Domain.Houses;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record NatalHouseSystemChoice(
    string Label,
    HouseSystem Value)
{
    public override string ToString()
        => Label;
}
