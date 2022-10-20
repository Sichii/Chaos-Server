using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record SoundArgs : ISendArgs
{
    public bool IsMusic { get; set; }
    public byte Sound { get; set; }
}