using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record CooldownArgs : ISendArgs
{
    public uint CooldownSecs { get; set; }
    public bool IsSkill { get; set; }
    public byte Slot { get; set; }
}