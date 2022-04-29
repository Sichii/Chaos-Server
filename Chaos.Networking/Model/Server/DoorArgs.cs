using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record DoorArgs : ISendArgs
{
    public ICollection<DoorArg> Doors { get; set; } = new List<DoorArg>();
}