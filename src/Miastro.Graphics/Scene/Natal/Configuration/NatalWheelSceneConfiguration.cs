namespace Miastro.Graphics.Scene.Natal.Configuration;

public sealed record NatalWheelSceneConfiguration(
    NatalWheelViewMode Mode,
    NatalWheelVisibilityOptions Visibility)
{
    public static NatalWheelSceneConfiguration
        ConsultationDefault =>
            new(
                NatalWheelViewMode.Consultation,
                new NatalWheelVisibilityOptions());

    public static NatalWheelSceneConfiguration
        PresentationDefault =>
            new(
                NatalWheelViewMode.Presentation,
                new NatalWheelVisibilityOptions(
                    ShowPlanets: true,
                    ShowPoints: true,
                    ShowAspects: true,
                    ShowCusps: true,
                    ShowLabels: false));
}
