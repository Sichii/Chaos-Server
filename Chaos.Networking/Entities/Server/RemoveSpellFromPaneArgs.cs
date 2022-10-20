using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record RemoveSpellFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}