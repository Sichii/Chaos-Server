using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record UserIdArgs : ISendArgs
{
    public BaseClass BaseClass { get; set; }
    public Direction Direction { get; set; }
    public Gender Gender { get; set; }
    public uint Id { get; set; }
}