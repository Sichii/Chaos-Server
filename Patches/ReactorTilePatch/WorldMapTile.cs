using Chaos.Geometry;

namespace ReactorTilePatch;

public sealed class WorldMapTile
{
    public Point Source { get; set; }
    public string WorldMapKey { get; set; } = null!;
}