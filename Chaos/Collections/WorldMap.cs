#region
using Chaos.Models.WorldMap;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents an in-game world map. A collection of world map nodes
/// </summary>
public sealed class WorldMap
{
    /// <summary>
    ///     The field index of the world map. This controls what image is displayed
    /// </summary>
    public required byte FieldIndex { get; init; }

    /// <summary>
    ///     A collection of world map nodes keyed by their unique id
    /// </summary>
    public required Dictionary<ushort, WorldMapNode> Nodes { get; init; } = new();

    /// <summary>
    ///     A unique string identifier for this world map
    /// </summary>
    public required string WorldMapKey { get; init; }
}