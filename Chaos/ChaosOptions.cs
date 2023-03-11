using Chaos.Common.Abstractions;

namespace Chaos;

public sealed class ChaosOptions : IStagingDirectory
{
    public bool LogRawPackets { get; init; }
    public string StagingDirectory { get; set; } = null!;
}