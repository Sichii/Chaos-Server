using Chaos.Common.Utilities;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed record AislingStoreOptions : IDirectoryBackupOptions
{
    public required string BackupDirectory { get; set; }
    public int BackupIntervalMins { get; set; }
    public int BackupRetentionDays { get; set; }

    public required string Directory { get; set; }

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory)
    {
        Directory = Path.Combine(baseDirectory, Directory);
        BackupDirectory = Path.Combine(baseDirectory, BackupDirectory);

        if (PathEx.IsSubPathOf(BackupDirectory, Directory))
            throw new InvalidOperationException($"{nameof(BackupDirectory)} cannot be a subdirectory of {nameof(Directory)}");
    }
}