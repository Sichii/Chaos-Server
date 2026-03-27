#region
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockDirectoryBackupServiceOptions : IDirectoryBackupOptions
{
    /// <inheritdoc />
    public string BackupDirectory { get; }

    /// <inheritdoc />
    public int BackupIntervalMins { get; }

    /// <inheritdoc />
    public int BackupRetentionDays { get; }

    /// <inheritdoc />
    public string Directory { get; }

    public MockDirectoryBackupServiceOptions(
        string directory,
        string backupDirectory,
        int backupIntervalMins,
        int backupRetentionDays)
    {
        Directory = directory;
        BackupDirectory = backupDirectory;
        BackupIntervalMins = backupIntervalMins;
        BackupRetentionDays = backupRetentionDays;
    }

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => throw new NotImplementedException();
}

public static class MockDirectoryBackupService
{
    public static Mock<DirectoryBackupService<MockDirectoryBackupServiceOptions>> Create(
        IOptions<MockDirectoryBackupServiceOptions> options,
        ILogger<DirectoryBackupService<MockDirectoryBackupServiceOptions>>? logger = null)
    {
        logger ??= MockLogger.Create<DirectoryBackupService<MockDirectoryBackupServiceOptions>>()
                             .Object;

        return new Mock<DirectoryBackupService<MockDirectoryBackupServiceOptions>>(options, logger)
        {
            CallBase = true
        };
    }

    public static Mock<DirectoryBackupService<MockDirectoryBackupServiceOptions>> Create(
        string directory,
        string backupDirectory,
        int backupIntervalMins,
        int backupRetentionDays,
        ILogger<DirectoryBackupService<MockDirectoryBackupServiceOptions>>? logger = null)
    {
        var options = new MockDirectoryBackupServiceOptions(
            directory,
            backupDirectory,
            backupIntervalMins,
            backupRetentionDays);

        var mockOptions = Options.Create(options);

        return Create(mockOptions, logger);
    }
}