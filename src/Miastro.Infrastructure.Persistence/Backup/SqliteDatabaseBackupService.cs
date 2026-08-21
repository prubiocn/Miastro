using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Miastro.Application.Backup;

namespace Miastro.Infrastructure.Persistence.Backup;

public sealed class SqliteDatabaseBackupService(
    MiastroDbContext dbContext)
    : IDatabaseBackupService
{
    public async Task BackupAsync(
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException(
                "Backup destination is required.",
                nameof(destinationPath));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var fullPath =
            Path.GetFullPath(destinationPath);

        var directory =
            Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "Unable to determine backup directory.");
        }

        Directory.CreateDirectory(directory);

        if (File.Exists(fullPath))
        {
            throw new IOException(
                "Backup destination already exists.");
        }

        var sourceConnection =
            (SqliteConnection)
                dbContext.Database.GetDbConnection();

        var shouldClose =
            sourceConnection.State
                != System.Data.ConnectionState.Open;

        if (shouldClose)
        {
            await sourceConnection.OpenAsync(
                cancellationToken);
        }

        try
        {
            await using var destination =
                new SqliteConnection(
                    new SqliteConnectionStringBuilder
                    {
                        DataSource = fullPath,
                        Mode = SqliteOpenMode.ReadWriteCreate
                    }.ToString());

            await destination.OpenAsync(
                cancellationToken);

            sourceConnection.BackupDatabase(
                destination);
        }
        finally
        {
            if (shouldClose)
            {
                await sourceConnection.CloseAsync();
            }
        }
    }
}
