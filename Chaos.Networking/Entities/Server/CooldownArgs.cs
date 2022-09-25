using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record CooldownArgs : ISendArgs
{
    public uint CooldownSecs { get; set; }
    public bool IsSkill { get; set; }
    public byte Slot { get; set; }
}