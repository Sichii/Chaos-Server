using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Data;

public record WarpSchema
{
    public required Location Destination { get; init; }
    public required Point Source { get; init; }
}