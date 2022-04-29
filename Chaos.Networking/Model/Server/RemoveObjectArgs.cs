using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record RemoveObjectArgs : ISendArgs
{
    public uint SourceId { get; set; }
}