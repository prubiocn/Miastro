using Miastro.Application.Natal;
using Miastro.Graphics.Adapters.Natal;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal.Configuration;
using Miastro.Graphics.Skia.Rendering;

namespace Miastro.UI.Avalonia.Services;

public sealed record NatalWheelPresentation(
    NatalScene Scene,
    byte[] PngBytes);

public sealed class NatalWheelPresentationService
{
    public NatalWheelPresentation Build(
        NatalChartSnapshotReadModel snapshot,
        double width,
        double height,
        NatalWheelSceneConfiguration? configuration = null,
        double renderScaling = 1.0)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!double.IsFinite(width)
            || !double.IsFinite(height)
            || !double.IsFinite(renderScaling)
            || width <= 0.0
            || height <= 0.0
            || renderScaling <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width));
        }

        var graphics =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    width,
                    height);

        var composed =
            new NatalWheelSceneComposer()
                .Compose(
                    graphics.Wheel,
                    graphics.Placements,
                    graphics.Objects,
                    graphics.Aspects,
                    configuration
                    ?? NatalWheelSceneConfiguration
                        .ConsultationDefault);

        var pixelWidth =
            Math.Max(
                1,
                (int)Math.Round(
                    width
                    * renderScaling));

        var pixelHeight =
            Math.Max(
                1,
                (int)Math.Round(
                    height
                    * renderScaling));

        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    composed.Scene,
                    pixelWidth,
                    pixelHeight);

        return new NatalWheelPresentation(
            composed.Scene,
            png);
    }
}
