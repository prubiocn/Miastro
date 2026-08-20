using Miastro.Infrastructure.Platform.Linux.Xdg;

namespace Miastro.Tests;

[TestClass]
public sealed class XdgApplicationPathsTests
{
    [TestMethod]
    public void Uses_custom_XDG_locations()
    {
        var root = CreateTemporaryRoot();

        WithXdgEnvironment(root, () =>
        {
            var paths = new XdgApplicationPaths();

            Assert.AreEqual(
                Path.Combine(root, "data", "miastro"),
                paths.DataDirectory);

            Assert.AreEqual(
                Path.Combine(root, "config", "miastro"),
                paths.ConfigDirectory);

            Assert.AreEqual(
                Path.Combine(root, "cache", "miastro"),
                paths.CacheDirectory);

            Assert.AreEqual(
                Path.Combine(root, "state", "miastro"),
                paths.StateDirectory);

            Assert.AreEqual(
                Path.Combine(root, "runtime", "miastro"),
                paths.RuntimeDirectory);

            Assert.IsTrue(Directory.Exists(paths.DataDirectory));
            Assert.IsTrue(Directory.Exists(paths.ConfigDirectory));
            Assert.IsTrue(Directory.Exists(paths.CacheDirectory));
            Assert.IsTrue(Directory.Exists(paths.StateDirectory));
            Assert.IsTrue(Directory.Exists(paths.RuntimeDirectory));
        });

        Directory.Delete(root, recursive: true);
    }

    private static string CreateTemporaryRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "miastro-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);
        return root;
    }

    internal static void WithXdgEnvironment(
        string root,
        Action action)
    {
        var names = new[]
        {
            "XDG_DATA_HOME",
            "XDG_CONFIG_HOME",
            "XDG_CACHE_HOME",
            "XDG_STATE_HOME",
            "XDG_RUNTIME_DIR"
        };

        var previous = names.ToDictionary(
            x => x,
            Environment.GetEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(
                "XDG_DATA_HOME",
                Path.Combine(root, "data"));

            Environment.SetEnvironmentVariable(
                "XDG_CONFIG_HOME",
                Path.Combine(root, "config"));

            Environment.SetEnvironmentVariable(
                "XDG_CACHE_HOME",
                Path.Combine(root, "cache"));

            Environment.SetEnvironmentVariable(
                "XDG_STATE_HOME",
                Path.Combine(root, "state"));

            Environment.SetEnvironmentVariable(
                "XDG_RUNTIME_DIR",
                Path.Combine(root, "runtime"));

            action();
        }
        finally
        {
            foreach (var item in previous)
            {
                Environment.SetEnvironmentVariable(
                    item.Key,
                    item.Value);
            }
        }
    }
}
