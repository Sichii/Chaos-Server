using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record DisplayVisibleObjectArgs : ISendArgs
{
    public ICollection<VisibleObjectInfo> VisibleObjects { get; set; } = new List<VisibleObjectInfo>();
}