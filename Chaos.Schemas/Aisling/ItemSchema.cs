using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Aisling.Abstractions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an item
/// </summary>
public sealed record ItemSchema : PanelEntitySchema
{
    /// <summary>
    ///     The color of this item if it's different that what is specified by it's template
    /// </summary>
    public DisplayColor? Color { get; set; }
    /// <summary>
    ///     The amount of this item in the stack
    /// </summary>
    [JsonRequired]
    public int Count { get; set; }
    /// <summary>
    ///     If the item has durability this is it's current durability
    /// </summary>
    public int? CurrentDurability { get; set; }
    /// <summary>
    ///     The display name for this item, if it's different than what is specified by it's template
    /// </summary>
    public string? DisplayName { get; set; }
    /// <summary>
    ///     The sprite to display for this item if it's different than what is specified by it's template
    /// </summary>
    public ushort? DisplaySprite { get; set; }
    /// <summary>
    ///     The sprite to display for this item in the panel if it's different than what is specified by it's template
    /// </summary>
    public ushort? PanelSprite { get; set; }
    /// <summary>
    ///     The weight of this item if it's different than what is specified by it's template
    /// </summary>
    public int? Weight { get; set; }
}