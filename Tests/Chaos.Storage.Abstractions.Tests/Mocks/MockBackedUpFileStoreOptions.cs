namespace Chaos.Storage.Abstractions.Tests.Mocks;

public sealed class MockBackedUpFileStoreOptions : IBackedUpFileStoreOptions
{
    /// <inheritdoc />
    public string BackupDirectory { get; set; } = null!;
    /// <inheritdoc />
    public int BackupIntervalMins { get; set; }
    /// <inheritdoc />
    public int BackupRetentionDays { get; set; }
    /// <inheritdoc />
    public string Directory { get; set; } = null!;

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => throw new NotImplementedException();
}