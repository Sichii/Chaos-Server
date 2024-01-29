using Chaos.Geometry.Abstractions;

namespace Chaos.Models.Data;

public class PathDetails
{
    public required Point Destination { get; set; }
    public required Stack<IPoint> Path { get; set; }
}