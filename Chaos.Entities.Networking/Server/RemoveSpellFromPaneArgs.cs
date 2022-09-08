using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record RemoveSpellFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}