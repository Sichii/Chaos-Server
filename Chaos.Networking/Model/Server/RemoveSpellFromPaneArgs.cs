using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record RemoveSpellFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}