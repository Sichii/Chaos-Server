using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record DoorArgs : ISendArgs
{
    public ICollection<DoorInfo> Doors { get; set; } = new List<DoorInfo>();
}