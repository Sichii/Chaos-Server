using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WorldMapWarpSchema
{
    public required string WorldMapKey { get; init; }
    public required Point Source { get; init; }
}