using Chaos.IO.FileSystem;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MailStoreOptions : IDirectoryBackupOptions, IPeriodicSaveStoreOptions
{
    /// <inheritdoc />
    public string BackupDirectory { get; set; } = null!;

    /// <inheritdoc />
    public int BackupIntervalMins { get; set; }

    /// <inheritdoc />
    public int BackupRetentionDays { get; set; }

    public string Directory { get; set; } = null!;
    public int SaveIntervalMins { get; set; }

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory)
    {
        Directory = Path.Combine(baseDirectory, Directory);
        BackupDirectory = Path.Combine(baseDirectory, BackupDirectory);

        if (PathEx.IsSubPathOf(BackupDirectory, Directory))
            throw new InvalidOperationException($"{nameof(BackupDirectory)} cannot be a subdirectory of {nameof(Directory)}");
    }
}