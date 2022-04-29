using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record DisplayVisibleObjectArgs : ISendArgs
{
    public ICollection<VisibleArg> VisibleObjects { get; set; } = new List<VisibleArg>();
}