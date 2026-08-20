using Miastro.Application.Platform;

namespace Miastro.Infrastructure.Platform.Linux.Xdg;

public sealed class XdgApplicationPaths : IApplicationPaths
{
    private const string ApplicationDirectoryName = "miastro";

    public XdgApplicationPaths()
    {
        var home = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile,
            Environment.SpecialFolderOption.DoNotVerify);

        if (string.IsNullOrWhiteSpace(home))
        {
            throw new InvalidOperationException(
                "No se pudo determinar el directorio personal del usuario.");
        }

        DataDirectory = Resolve(
            "XDG_DATA_HOME",
            Path.Combine(home, ".local", "share"),
            ApplicationDirectoryName);

        ConfigDirectory = Resolve(
            "XDG_CONFIG_HOME",
            Path.Combine(home, ".config"),
            ApplicationDirectoryName);

        CacheDirectory = Resolve(
            "XDG_CACHE_HOME",
            Path.Combine(home, ".cache"),
            ApplicationDirectoryName);

        StateDirectory = Resolve(
            "XDG_STATE_HOME",
            Path.Combine(home, ".local", "state"),
            ApplicationDirectoryName);

        RuntimeDirectory = ResolveRuntimeDirectory();

        DatabasePath = Path.Combine(DataDirectory, "miastro.db");
        SettingsPath = Path.Combine(ConfigDirectory, "settings.json");
        LogDirectory = Path.Combine(StateDirectory, "logs");
        LogFilePath = Path.Combine(LogDirectory, "miastro.log");

        EnsureDirectories();
    }

    public string DataDirectory { get; }

    public string ConfigDirectory { get; }

    public string CacheDirectory { get; }

    public string StateDirectory { get; }

    public string RuntimeDirectory { get; }

    public string DatabasePath { get; }

    public string SettingsPath { get; }

    public string LogDirectory { get; }

    public string LogFilePath { get; }

    private static string Resolve(
        string environmentVariable,
        string fallbackRoot,
        string applicationDirectory)
    {
        var configured = Environment.GetEnvironmentVariable(environmentVariable);

        var root = !string.IsNullOrWhiteSpace(configured) &&
                   Path.IsPathRooted(configured)
            ? configured
            : fallbackRoot;

        return Path.Combine(root, applicationDirectory);
    }

    private static string ResolveRuntimeDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");

        if (!string.IsNullOrWhiteSpace(configured) &&
            Path.IsPathRooted(configured))
        {
            return Path.Combine(configured, ApplicationDirectoryName);
        }

        return Path.Combine(
            Path.GetTempPath(),
            $"miastro-{Environment.UserName}");
    }

    private void EnsureDirectories()
    {
        EnsurePrivateDirectory(DataDirectory);
        EnsurePrivateDirectory(ConfigDirectory);
        EnsurePrivateDirectory(CacheDirectory);
        EnsurePrivateDirectory(StateDirectory);
        EnsurePrivateDirectory(RuntimeDirectory);
        EnsurePrivateDirectory(LogDirectory);
    }

    private static void EnsurePrivateDirectory(string path)
    {
        Directory.CreateDirectory(path);

        if (OperatingSystem.IsLinux())
        {
            File.SetUnixFileMode(
                path,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite |
                UnixFileMode.UserExecute);
        }
    }
}
