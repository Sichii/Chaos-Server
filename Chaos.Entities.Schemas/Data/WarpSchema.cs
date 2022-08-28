using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Data;

public record WarpSchema
{
    public Location Destination { get; init; }
    public Point Source { get; init; }
}