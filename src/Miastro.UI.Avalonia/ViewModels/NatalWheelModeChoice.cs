using Miastro.Graphics.Scene.Natal.Configuration;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record NatalWheelModeChoice(
    string Label,
    NatalWheelViewMode Value)
{
    public override string ToString()
        => Label;
}
