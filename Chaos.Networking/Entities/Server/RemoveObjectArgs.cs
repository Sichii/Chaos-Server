using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record RemoveObjectArgs : ISendArgs
{
    public uint SourceId { get; set; }
}