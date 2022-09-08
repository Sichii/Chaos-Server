using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record DisplayVisibleObjectArgs : ISendArgs
{
    public ICollection<VisibleObjectInfo> VisibleObjects { get; set; } = new List<VisibleObjectInfo>();
}