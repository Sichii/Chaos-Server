using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record PublicMessageArgs : ISendArgs
{
    public string Message { get; set; } = null!;
    public PublicMessageType PublicMessageType { get; set; }
    public uint SourceId { get; set; }
}