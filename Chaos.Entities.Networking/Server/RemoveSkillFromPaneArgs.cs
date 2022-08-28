using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record RemoveSkillFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}