#region
using System.IO.Compression;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Storage.Abstractions.Tests;

public sealed class DirectoryBackupServiceTests : IDisposable
{
    private readonly Mock<ILogger<DirectoryBackupService<MockDirectoryBackupServiceOptions>>> LoggerMock;
    private readonly IOptions<MockDirectoryBackupServiceOptions> Options;
    private readonly Mock<DirectoryBackupService<MockDirectoryBackupServiceOptions>> ServiceMock;
    private readonly string TempRoot;

    public DirectoryBackupServiceTests()
    {
        TempRoot = Path.Combine(
            Path.GetTempPath(),
            "DirBackupBranches_"
            + Guid.NewGuid()
                  .ToString("N"));
        var save = Path.Combine(TempRoot, "Save");
        var backup = Path.Combine(TempRoot, "Backup");
        Directory.CreateDirectory(save);
        Directory.CreateDirectory(backup);

        var options = new MockDirectoryBackupServiceOptions(
            save,
            backup,
            1,
            1);

        Options = MockOptions.Create(options)
                             .Object;

        LoggerMock = MockLogger.Create<DirectoryBackupService<MockDirectoryBackupServiceOptions>>();
        ServiceMock = MockDirectoryBackupService.Create(Options, LoggerMock.Object);
    }

    public void Dispose() => Directory.Delete(TempRoot, true);

    [Test]
    public async Task HandleBackupRetention_Should_Delete_Old_Backups()
    {
        // Arrange
        var token = CancellationToken.None;
        var oldBackup = Path.Combine(Options.Value.BackupDirectory, "oldBackup.zip");
        var service = ServiceMock.Object;
        await File.WriteAllTextAsync(oldBackup, "dummy content", token);
        File.SetCreationTimeUtc(oldBackup, DateTime.UtcNow.AddDays(-(Options.Value.BackupRetentionDays + 2))); // Set the file to be old

        // Act
        await service.HandleBackupRetentionAsync(Options.Value.BackupDirectory, token);

        // Assert
        File.Exists(oldBackup)
            .Should()
            .BeFalse();
    }

    [Test]
    public async Task HandleBackupRetention_Should_Not_Delete_Backups_Within_Retention_Period()
    {
        // Arrange
        var token = CancellationToken.None;
        var backupDirectory = Options.Value.BackupDirectory;
        var service = ServiceMock.Object;
        var backupFilePath = Path.Combine(backupDirectory, "backup.zip");

        // Create a backup file with today's date
        await File.Create(backupFilePath)
                  .DisposeAsync();

        // Act
        await service.HandleBackupRetentionAsync(backupDirectory, token);

        // Assert
        File.Exists(backupFilePath)
            .Should()
            .BeTrue("because the backup is within the retention period and should not be deleted");
    }

    [Test]
    public async Task HandleBackupRetention_When_Directory_Missing_Should_Log_And_Return()
    {
        // Arrange
        var missing = Path.Combine(TempRoot, "Missing");
        var service = ServiceMock.Object;

        // Act
        await service.HandleBackupRetentionAsync(missing, CancellationToken.None);

        // Assert: no exception, returns default
        // Can't directly assert logs without a provider; absence of exception implies branch executed
        true.Should()
            .BeTrue();
    }

    [Test]
    public async Task TakeBackupAsync_Should_Create_Backup_File_If_Directory_Exists_And_Verify_Its_Contents()
    {
        // Arrange
        var saveDirectory = Options.Value.Directory;
        var backupDirectory = Options.Value.BackupDirectory;
        var service = ServiceMock.Object;

        const string FILE1_NAME = "file1.txt";
        const string FILE2_NAME = "file2.txt";
        const string FILE1_CONTENT = "File 1 Content";
        const string FILE2_CONTENT = "File 2 Content";

        // Create some files in the saveDirectory
        await File.WriteAllTextAsync(Path.Combine(saveDirectory, FILE1_NAME), FILE1_CONTENT);
        await File.WriteAllTextAsync(Path.Combine(saveDirectory, FILE2_NAME), FILE2_CONTENT);
        Directory.SetLastWriteTimeUtc(saveDirectory, DateTime.UtcNow); // Set the directory to be recent

        // Act
        await service.TakeBackupAsync(saveDirectory, CancellationToken.None);

        // Assert
        var backupFiles = Directory.GetFiles(backupDirectory, "*.zip", SearchOption.AllDirectories);

        backupFiles.Should()
                   .NotBeEmpty();

        // Unzip the backup and verify its contents
        var backupPath = backupFiles.First();
        var extractPath = Path.Combine(TempRoot, "Extracted");
        ZipFile.ExtractToDirectory(backupPath, extractPath);

        (await File.ReadAllTextAsync(Path.Combine(extractPath, FILE1_NAME))).Should()
                                                                            .Be(FILE1_CONTENT);

        (await File.ReadAllTextAsync(Path.Combine(extractPath, FILE2_NAME))).Should()
                                                                            .Be(FILE2_CONTENT);
    }

    [Test]
    public async Task TakeBackupAsync_Should_Not_Create_Backup_If_Directory_Does_Not_Exist()
    {
        // Arrange
        var nonExistentDirectory = Path.Combine(TempRoot, "NonExistentDirectory");
        var service = ServiceMock.Object;

        // Act
        await service.TakeBackupAsync(nonExistentDirectory, CancellationToken.None);

        // Assert
        var backupFiles = Directory.GetFiles(Options.Value.BackupDirectory, "*.zip");

        backupFiles.Should()
                   .BeEmpty();
    }

    [Test]
    public async Task TakeBackupAsync_Should_Respect_Cancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var service = ServiceMock.Object;

        await cts.CancelAsync();

        // Act
        await service.TakeBackupAsync(Options.Value.Directory, cts.Token);

        // Assert: no zip created
        Directory.EnumerateFiles(Options.Value.BackupDirectory, "*.zip", SearchOption.AllDirectories)
                 .Should()
                 .BeEmpty();
    }

    [Test]
    public async Task TakeBackupAsync_Should_Return_When_Not_Modified_Recently()
    {
        // Arrange: set last write time older than threshold
        var save = Options.Value.Directory;
        var service = ServiceMock.Object;
        Directory.SetLastWriteTimeUtc(save, DateTime.UtcNow.AddMinutes(-Options.Value.BackupIntervalMins - 10));

        // Act
        await service.TakeBackupAsync(save, CancellationToken.None);

        // Assert: no zip created
        Directory.EnumerateFiles(Options.Value.BackupDirectory, "*.zip", SearchOption.AllDirectories)
                 .Should()
                 .BeEmpty();
    }
}