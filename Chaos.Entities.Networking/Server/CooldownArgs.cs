using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record CooldownArgs : ISendArgs
{
    public uint CooldownSecs { get; set; }
    public bool IsSkill { get; set; }
    public byte Slot { get; set; }
}