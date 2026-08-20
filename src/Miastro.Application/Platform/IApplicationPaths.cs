namespace Miastro.Application.Platform;

public interface IApplicationPaths
{
    string DataDirectory { get; }
    string ConfigDirectory { get; }
    string CacheDirectory { get; }
    string StateDirectory { get; }
    string RuntimeDirectory { get; }

    string DatabasePath { get; }
    string SettingsPath { get; }
    string LogDirectory { get; }
    string LogFilePath { get; }
}
