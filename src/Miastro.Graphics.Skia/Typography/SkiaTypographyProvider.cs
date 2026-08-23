using System.Reflection;
using SkiaSharp;

namespace Miastro.Graphics.Skia.Typography;

public sealed class SkiaTypographyProvider : IDisposable
{
    private const string FontResourceName =
        "Miastro.Graphics.Skia.Resources.Fonts.SourceSans3-Regular.ttf";

    private readonly SKTypeface _typeface;

    public SkiaTypographyProvider()
    {
        var assembly =
            typeof(SkiaTypographyProvider)
                .Assembly;

        using var stream =
            assembly.GetManifestResourceStream(
                FontResourceName)
            ?? throw new InvalidOperationException(
                $"Embedded font resource '{FontResourceName}' was not found.");

        _typeface =
            SKTypeface.FromStream(stream)
            ?? throw new InvalidOperationException(
                "Unable to load embedded Source Sans 3 font.");
    }

    public SKTypeface Typeface =>
        _typeface;

    public string FamilyName =>
        _typeface.FamilyName;

    public void Dispose()
    {
        _typeface.Dispose();
    }
}
