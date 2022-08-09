using Chaos.Geometry.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record UserIdArgs : ISendArgs
{
    public BaseClass BaseClass { get; set; }
    public Direction Direction { get; set; }
    public Gender Gender { get; set; }
    public uint Id { get; set; }
}