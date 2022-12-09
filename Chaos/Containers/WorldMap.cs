using Chaos.Data;

namespace Chaos.Containers;

public sealed class WorldMap
{
    public required byte FieldIndex { get; init; }
    public required Dictionary<ushort, WorldMapNode> Nodes { get; init; } = new();
    public required string WorldMapKey { get; init; }
}