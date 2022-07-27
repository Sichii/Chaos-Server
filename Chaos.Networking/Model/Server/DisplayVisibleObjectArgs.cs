using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record DisplayVisibleObjectArgs : ISendArgs
{
    public ICollection<VisibleObjectInfo> VisibleObjects { get; set; } = new List<VisibleObjectInfo>();
}