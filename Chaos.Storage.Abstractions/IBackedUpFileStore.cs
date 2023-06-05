using Microsoft.Extensions.Hosting;

namespace Chaos.Storage.Abstractions;

/// <summary>
///     Provides the methods required to manage a file store that takes backups
/// </summary>
public interface IBackedUpFileStore : IHostedService
{
    /// <summary>
    ///     Handles the retention of backups in the specified directory
    /// </summary>
    /// <param name="directory">The directory to prune backups from</param>
    void HandleBackupRetention(string directory);

    /// <summary>
    ///     Takes a backup of the specified directory
    /// </summary>
    /// <param name="saveDirectory">The directory to take a backup of</param>
    /// <param name="token">A token used to cancel the action</param>
    ValueTask TakeBackupAsync(string saveDirectory, CancellationToken token);
}