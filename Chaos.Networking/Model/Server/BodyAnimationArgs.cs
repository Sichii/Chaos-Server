using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record BodyAnimationArgs : ISendArgs
{
    public BodyAnimation BodyAnimation { get; set; }
    public byte Sound { get; set; }
    public uint SourceId { get; set; }
    public ushort Speed { get; set; }
}