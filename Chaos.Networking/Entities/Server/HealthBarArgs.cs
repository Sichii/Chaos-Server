using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record HealthBarArgs : ISendArgs
{
    public byte HealthPercent { get; set; }
    public byte Sound { get; set; }
    public uint SourceId { get; set; }
}