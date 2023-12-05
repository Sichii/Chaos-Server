using System.IO.Compression;
using Chaos.Storage.Abstractions.Tests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Chaos.Storage.Abstractions.Tests;

public sealed class DirectoryBackupServiceTests : IDisposable
{
    private readonly MockDirectoryBackupService BackupService;
    private readonly Mock<ILogger<MockDirectoryBackupService>> LoggerMock;
    private readonly IOptions<IDirectoryBackupOptions> Options;
    private readonly string TestDirectory;

    public DirectoryBackupServiceTests()
    {
        LoggerMock = new Mock<ILogger<MockDirectoryBackupService>>();
        TestDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        Options = Microsoft.Extensions.Options.Options.Create<IDirectoryBackupOptions>(
            new MockDirectoryBackupOptions
            {
                Directory = Path.Combine(TestDirectory, "ExistingDirectory"),
                BackupDirectory = Path.Combine(TestDirectory, "BackupDirectory"),
                BackupIntervalMins = 1,
                BackupRetentionDays = 1
            });

        BackupService = new MockDirectoryBackupService(Options, LoggerMock.Object);
        Directory.CreateDirectory(Options.Value.Directory);
        Directory.CreateDirectory(Options.Value.BackupDirectory);
    }

    public void Dispose()
        =>

            // Clean up test directory after each test run
            Directory.Delete(TestDirectory, true);

    [Fact]
    public async Task HandleBackupRetention_Should_Delete_Old_Backups()
    {
        // Arrange
        var token = new CancellationToken();
        var oldBackup = Path.Combine(Options.Value.BackupDirectory, "oldBackup.zip");
        await File.WriteAllTextAsync(oldBackup, "dummy content", token);
        File.SetCreationTimeUtc(oldBackup, DateTime.UtcNow.AddDays(-Options.Value.BackupRetentionDays - 2)); // Set the file to be old

        // Act
        await BackupService.HandleBackupRetentionAsync(Options.Value.BackupDirectory, token);

        // Assert
        File.Exists(oldBackup)
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task HandleBackupRetention_Should_Not_Delete_Backups_Within_Retention_Period()
    {
        // Arrange
        var token = new CancellationToken();
        var backupDirectory = Options.Value.BackupDirectory;

        var backupFilePath = Path.Combine(backupDirectory, "backup.zip");

        // Create a backup file with today's date
        await File.Create(backupFilePath)
                  .DisposeAsync();

        // Act
        await BackupService.HandleBackupRetentionAsync(backupDirectory, token);

        // Assert
        File.Exists(backupFilePath)
            .Should()
            .BeTrue("because the backup is within the retention period and should not be deleted");
    }

    [Fact]
    public async Task TakeBackupAsync_Should_Create_Backup_File_If_Directory_Exists_And_Verify_Its_Contents()
    {
        // Arrange
        var saveDirectory = Options.Value.Directory;
        var backupDirectory = Options.Value.BackupDirectory;

        const string FILE1_NAME = "file1.txt";
        const string FILE2_NAME = "file2.txt";
        const string FILE1_CONTENT = "File 1 Content";
        const string FILE2_CONTENT = "File 2 Content";

        // Create some files in the saveDirectory
        await File.WriteAllTextAsync(Path.Combine(saveDirectory, FILE1_NAME), FILE1_CONTENT);
        await File.WriteAllTextAsync(Path.Combine(saveDirectory, FILE2_NAME), FILE2_CONTENT);
        Directory.SetLastWriteTimeUtc(saveDirectory, DateTime.UtcNow); // Set the directory to be recent

        // Act
        await BackupService.TakeBackupAsync(saveDirectory, CancellationToken.None);

        // Assert
        var backupFiles = Directory.GetFiles(backupDirectory, "*.zip", SearchOption.AllDirectories);

        backupFiles.Should()
                   .NotBeEmpty();

        // Unzip the backup and verify its contents
        var backupPath = backupFiles.First();
        var extractPath = Path.Combine(TestDirectory, "Extracted");
        ZipFile.ExtractToDirectory(backupPath, extractPath);

        (await File.ReadAllTextAsync(Path.Combine(extractPath, FILE1_NAME))).Should()
                                                                            .Be(FILE1_CONTENT);

        (await File.ReadAllTextAsync(Path.Combine(extractPath, FILE2_NAME))).Should()
                                                                            .Be(FILE2_CONTENT);
    }

    [Fact]
    public async Task TakeBackupAsync_Should_Not_Create_Backup_If_Directory_Does_Not_Exist()
    {
        // Arrange
        var nonExistentDirectory = Path.Combine(TestDirectory, "NonExistentDirectory");

        // Act
        await BackupService.TakeBackupAsync(nonExistentDirectory, CancellationToken.None);

        // Assert
        var backupFiles = Directory.GetFiles(Options.Value.BackupDirectory, "*.zip");

        backupFiles.Should()
                   .BeEmpty();
    }
}