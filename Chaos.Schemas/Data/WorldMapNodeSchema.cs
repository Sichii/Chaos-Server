using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WorldMapNodeSchema
{
    public required string NodeKey { get; init; }
    public required Location Destination { get; init; }
    public required string Text { get; init; }
    public required Point ScreenPosition { get; init; }
}