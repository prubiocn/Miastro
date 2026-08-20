using Microsoft.Extensions.Logging;

namespace Miastro.Infrastructure.Platform.Linux.Logging;

public sealed class XdgFileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;
    private readonly object _sync = new();

    public XdgFileLoggerProvider(string logFilePath)
    {
        _logFilePath = logFilePath;

        var directory = Path.GetDirectoryName(_logFilePath)
            ?? throw new InvalidOperationException(
                "La ruta del log no contiene directorio.");

        Directory.CreateDirectory(directory);
    }

    public ILogger CreateLogger(string categoryName) =>
        new XdgFileLogger(categoryName, _logFilePath, _sync);

    public void Dispose()
    {
    }

    private sealed class XdgFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;
        private readonly object _sync;

        public XdgFileLogger(
            string categoryName,
            string logFilePath,
            object sync)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _sync = sync;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            var line =
                $"{DateTimeOffset.UtcNow:O} " +
                $"[{logLevel}] {_categoryName}: {message}";

            lock (_sync)
            {
                File.AppendAllText(
                    _logFilePath,
                    line + Environment.NewLine);

                if (OperatingSystem.IsLinux())
                {
                    File.SetUnixFileMode(
                        _logFilePath,
                        UnixFileMode.UserRead |
                        UnixFileMode.UserWrite);
                }
            }
        }
    }
}
