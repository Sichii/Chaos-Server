using Chaos.Common.Abstractions;

namespace Chaos;

public class ChaosOptions : IStagingDirectory
{
    public string StagingDirectory { get; init; } = null!;
}