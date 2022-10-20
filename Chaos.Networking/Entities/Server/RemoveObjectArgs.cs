using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record RemoveObjectArgs : ISendArgs
{
    public uint SourceId { get; set; }
}