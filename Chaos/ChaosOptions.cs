#region
using Chaos.Common.Abstractions;
#endregion

namespace Chaos;

public sealed class ChaosOptions : IStagingDirectory
{
    public bool LogRawPackets { get; init; }
    public bool LogReceivePacketCode { get; init; }
    public bool LogSendPacketCode { get; init; }
    public string StagingDirectory { get; set; } = null!;
}