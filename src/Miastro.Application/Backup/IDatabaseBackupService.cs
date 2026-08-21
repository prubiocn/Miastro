namespace Miastro.Application.Backup;

public interface IDatabaseBackupService
{
    Task BackupAsync(
        string destinationPath,
        CancellationToken cancellationToken = default);
}
