using Chaos.Common.Abstractions;

namespace Chaos;

public sealed class ChaosOptions : IStagingDirectory
{
    public string StagingDirectory { get; init; } = null!;
}