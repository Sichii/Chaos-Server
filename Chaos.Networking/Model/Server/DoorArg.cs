using Chaos.Core.Geometry;

namespace Chaos.Networking.Model.Server;

public record DoorArg
{
    public bool Closed { get; set; }
    public bool OpenRight { get; set; }
    public Point Point { get; set; }
}