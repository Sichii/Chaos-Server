using Chaos.Common.Abstractions;

namespace Chaos.Storage.Abstractions;

/// <summary>
///     Provides the options required for a <see cref="IBackedUpFileStore{T}" />
/// </summary>
public interface IBackedUpFileStoreOptions : IDirectoryBound
{
    /// <summary>
    ///     The directory where backups are stored
    /// </summary>
    string BackupDirectory { get; }
    /// <summary>
    ///     The interval between backups in minutes
    /// </summary>
    int BackupIntervalMins { get; }
    /// <summary>
    ///     The number of days to retain backups
    /// </summary>
    int BackupRetentionDays { get; }
    /// <summary>
    ///     The directory where objects are stored
    /// </summary>
    string Directory { get; }
}