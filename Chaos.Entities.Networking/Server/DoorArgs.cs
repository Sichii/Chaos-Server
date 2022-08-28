using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record DoorArgs : ISendArgs
{
    public ICollection<DoorInfo> Doors { get; set; } = new List<DoorInfo>();
}