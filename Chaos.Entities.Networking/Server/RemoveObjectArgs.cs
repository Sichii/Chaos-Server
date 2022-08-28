using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record RemoveObjectArgs : ISendArgs
{
    public uint SourceId { get; set; }
}