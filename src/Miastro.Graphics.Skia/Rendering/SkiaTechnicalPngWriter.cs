using Miastro.Graphics.Scene;

namespace Miastro.Graphics.Skia.Rendering;

public sealed class SkiaTechnicalPngWriter(
    SkiaNatalSceneRenderer renderer)
{
    public void Write(
        NatalScene scene,
        string path,
        int pixelWidth,
        int pixelHeight)
    {
        ArgumentNullException.ThrowIfNull(
            scene);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(
                "Output path is required.",
                nameof(path));
        }

        var fullPath =
            Path.GetFullPath(
                path);

        var directory =
            Path.GetDirectoryName(
                fullPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(
                directory);
        }

        var bytes =
            renderer.RenderPng(
                scene,
                pixelWidth,
                pixelHeight);

        File.WriteAllBytes(
            fullPath,
            bytes);
    }
}
