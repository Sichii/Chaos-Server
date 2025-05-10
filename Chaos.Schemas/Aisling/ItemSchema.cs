#region
using System.Text.Json.Serialization;
using Chaos.DarkAges.Definitions;
using Chaos.Schemas.Aisling.Abstractions;
#endregion

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an item
/// </summary>
public sealed record ItemSchema : PanelEntitySchema
{
    /// <summary>
    ///     Whether or not the item is account bound. If the item is account bound, it cannot be traded or dropped
    /// </summary>
    public bool? AccountBound { get; set; }

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
    ///     If specified, the custom name of the item. This name overrides prefix, suffix, and color
    /// </summary>
    public string? CustomNameOverride { get; set; }

    /// <summary>
    ///     The sprite to display for this item if it's different than what is specified by it's template
    /// </summary>
    public ushort? DisplaySprite { get; set; }

    /// <summary>
    ///     The text of the notepad attached to this item
    /// </summary>
    public string? NotepadText { get; set; }

    /// <summary>
    ///     Whether or not the item can be traded
    /// </summary>
    public bool? NoTrade { get; set; }

    /// <summary>
    ///     The sprite to display for this item in the panel if it's different than what is specified by it's template
    /// </summary>
    public ushort? PanelSprite { get; set; }

    /// <summary>
    ///     If specified, the prefix of the item
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    ///     Whether or not the item can be banked
    /// </summary>
    public bool? PreventBanking { get; set; }

    /// <summary>
    ///     If specified, the suffix of the item
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    ///     The weight of this item if it's different than what is specified by it's template
    /// </summary>
    public int? Weight { get; set; }
}