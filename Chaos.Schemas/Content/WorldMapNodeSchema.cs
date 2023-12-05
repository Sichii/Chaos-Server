using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a world map node
/// </summary>
public sealed record WorldMapNodeSchema
{
    /// <summary>
    ///     The location this node leads to when clicked
    /// </summary>
    [JsonRequired]
    public Location Destination { get; set; } = null!;

    /// <summary>
    ///     A unique id specific to this node
    /// </summary>
    [JsonRequired]
    public string NodeKey { get; set; } = null!;

    /// <summary>
    ///     The point on the screen this node will be rendered at
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point ScreenPosition { get; set; }

    /// <summary>
    ///     The text displayed next to this node
    /// </summary>
    [JsonRequired]
    public string Text { get; set; } = null!;
}