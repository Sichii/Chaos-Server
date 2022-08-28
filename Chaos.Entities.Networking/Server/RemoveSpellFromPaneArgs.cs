using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record RemoveSpellFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}