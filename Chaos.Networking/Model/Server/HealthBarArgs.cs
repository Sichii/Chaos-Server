using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record HealthBarArgs : ISendArgs
{
    public byte HealthPercent { get; set; }
    public byte Sound { get; set; }
    public uint SourceId { get; set; }
}