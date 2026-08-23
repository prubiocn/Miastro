namespace Miastro.Graphics.Styles;

public sealed record SceneStyle(
    string Key,
    SceneColor StrokeColor,
    double StrokeWidth,
    SceneLinePattern LinePattern = SceneLinePattern.Solid,
    double Opacity = 1.0,
    SceneColor? FillColor = null)
{
    public SceneStyle Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new ArgumentException(
                "Style key is required.",
                nameof(Key));
        }

        if (!double.IsFinite(StrokeWidth)
            || StrokeWidth < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(StrokeWidth));
        }

        if (!double.IsFinite(Opacity)
            || Opacity < 0.0
            || Opacity > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Opacity));
        }

        return this;
    }
}
