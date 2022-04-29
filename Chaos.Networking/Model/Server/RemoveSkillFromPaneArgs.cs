using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record RemoveSkillFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}