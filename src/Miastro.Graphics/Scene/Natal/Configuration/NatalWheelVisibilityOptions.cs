namespace Miastro.Graphics.Scene.Natal.Configuration;

public sealed record NatalWheelVisibilityOptions(
    bool ShowPlanets = true,
    bool ShowPoints = true,
    bool ShowAspects = true,
    bool ShowCusps = true,
    bool ShowLabels = true);
