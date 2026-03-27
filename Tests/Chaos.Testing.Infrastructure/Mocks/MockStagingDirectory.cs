using Chaos.Common.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockStagingDirectory : IStagingDirectory
{
    public string StagingDirectory { get; set; }

    public MockStagingDirectory(string? basePath = null)
    {
        StagingDirectory = basePath ?? Path.Combine(Path.GetTempPath(), "ChaosTests_Stage");
        Directory.CreateDirectory(StagingDirectory);
    }

    public static Mock<IStagingDirectory> Create(string? basePath = null, Action<Mock<IStagingDirectory>>? setup = null)
    {
        var path = basePath ?? Path.Combine(Path.GetTempPath(), "ChaosTests_Stage");
        Directory.CreateDirectory(path);
        var mock = new Mock<IStagingDirectory>();
        mock.SetupProperty(s => s.StagingDirectory, path);
        setup?.Invoke(mock);

        return mock;
    }
}