using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record DoorArgs : ISendArgs
{
    public ICollection<DoorInfo> Doors { get; set; } = new List<DoorInfo>();
}