#region
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Storage.Tests;

public sealed class DirectoryBackupServiceTests : IDisposable
{
    private readonly string BackupDir;
    private readonly string TempDir;

    public DirectoryBackupServiceTests()
    {
        TempDir = Path.Combine(Path.GetTempPath(), $"DirBackup_Src_{Guid.NewGuid():N}");
        BackupDir = Path.Combine(Path.GetTempPath(), $"DirBackup_Bak_{Guid.NewGuid():N}");
        Directory.CreateDirectory(TempDir);
        Directory.CreateDirectory(BackupDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(TempDir))
            Directory.Delete(TempDir, true);

        if (Directory.Exists(BackupDir))
            Directory.Delete(BackupDir, true);
    }

    [Test]
    public async Task HandleBackupRetentionAsync_DeletesExpiredFiles()
    {
        // Create a file with a creation time older than retention days
        var oldFile = Path.Combine(BackupDir, "old_backup.zip");
        File.WriteAllText(oldFile, "old data");
        File.SetCreationTimeUtc(oldFile, DateTime.UtcNow.AddDays(-10));

        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                60,
                                                7)
                                            .Object;

        await svc.HandleBackupRetentionAsync(BackupDir, CancellationToken.None);

        File.Exists(oldFile)
            .Should()
            .BeFalse("files older than retention period should be deleted");
    }

    [Test]
    public async Task HandleBackupRetentionAsync_NonExistentDirectory_DoesNotThrow()
    {
        var nonExistent = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid():N}");

        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                60,
                                                7)
                                            .Object;

        var act = async () => await svc.HandleBackupRetentionAsync(nonExistent, CancellationToken.None);

        await act.Should()
                 .NotThrowAsync();
    }

    [Test]
    public async Task HandleBackupRetentionAsync_PreservesRecentFiles()
    {
        // Create a file with creation time within retention window
        var newFile = Path.Combine(BackupDir, "recent_backup.zip");
        File.WriteAllText(newFile, "recent data");
        File.SetCreationTimeUtc(newFile, DateTime.UtcNow.AddDays(-1));

        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                60,
                                                7)
                                            .Object;

        await svc.HandleBackupRetentionAsync(BackupDir, CancellationToken.None);

        File.Exists(newFile)
            .Should()
            .BeTrue("files within retention period should be preserved");
    }

    [Test]
    public async Task TakeBackupAsync_CreatesZipForRecentlyModifiedDirectory()
    {
        var saveDir = Path.Combine(TempDir, "player1");
        Directory.CreateDirectory(saveDir);
        File.WriteAllText(Path.Combine(saveDir, "data.json"), "{\"hp\":100}");
        Directory.SetLastWriteTimeUtc(saveDir, DateTime.UtcNow);

        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                60,
                                                7)
                                            .Object;

        await svc.TakeBackupAsync(saveDir, CancellationToken.None);

        var zips = Directory.EnumerateFiles(BackupDir, "*.zip", SearchOption.AllDirectories)
                            .ToList();

        zips.Should()
            .HaveCount(1, "a backup zip should be created for a recently modified directory");
    }

    [Test]
    public async Task TakeBackupAsync_NonExistentDirectory_DoesNotThrow()
    {
        var nonExistent = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid():N}");

        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                60,
                                                7)
                                            .Object;

        var act = async () => await svc.TakeBackupAsync(nonExistent, CancellationToken.None);

        await act.Should()
                 .NotThrowAsync();
    }

    [Test]
    public async Task TakeBackupAsync_SkipsBackupForStaleDirectory()
    {
        var saveDir = Path.Combine(TempDir, "player2");
        Directory.CreateDirectory(saveDir);
        File.WriteAllText(Path.Combine(saveDir, "data.json"), "{\"hp\":50}");

        // Set last write time far in the past so it's before the interval threshold
        Directory.SetLastWriteTimeUtc(saveDir, DateTime.UtcNow.AddDays(-2));

        // Use a 1-minute interval: last write was 2 days ago → condition: lastWrite < now - 1min → skip
        var svc = MockDirectoryBackupService.Create(
                                                TempDir,
                                                BackupDir,
                                                1,
                                                7)
                                            .Object;

        await svc.TakeBackupAsync(saveDir, CancellationToken.None);

        var zips = Directory.EnumerateFiles(BackupDir, "*.zip", SearchOption.AllDirectories)
                            .ToList();

        zips.Should()
            .BeEmpty("no backup should be created for a directory that hasn't been modified recently");
    }
}