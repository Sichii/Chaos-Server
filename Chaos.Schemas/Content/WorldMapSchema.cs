using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a world map
/// </summary>
public sealed class WorldMapSchema
{
    /// <summary>
    ///     The index of the map image to use when displaying this world map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte FieldIndex { get; set; }

    /// <summary>
    ///     A collection of keys for the nodes that are displayed on this world map
    /// </summary>
    public ICollection<string> NodeKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A unique id specific to this world map
    /// </summary>
    [JsonRequired]
    public string WorldMapKey { get; set; } = null!;
}