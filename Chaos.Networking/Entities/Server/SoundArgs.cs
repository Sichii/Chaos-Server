using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record SoundArgs : ISendArgs
{
    public bool IsMusic { get; set; }
    public byte Sound { get; set; }
}