using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record BodyAnimationArgs : ISendArgs
{
    public BodyAnimation BodyAnimation { get; set; }
    public byte Sound { get; set; }
    public uint SourceId { get; set; }
    public ushort Speed { get; set; }
}