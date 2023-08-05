using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates.Abstractions;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for an item template
/// </summary>
public sealed record ItemTemplateSchema : PanelEntityTemplateSchema
{
    /// <summary>
    ///     Whether or not the item is account bound. If the item is account bound, it cannot be traded or dropped
    /// </summary>
    public bool AccountBound { get; set; }

    /// <summary>
    ///     The amount of gold it costs to buy this item from a merchant
    /// </summary>
    public int BuyCost { get; set; }
    /// <summary>
    ///     The category of the item, used for bank or shop sorting
    /// </summary>
    [JsonRequired]
    public string Category { get; set; } = null!;

    /// <summary>
    ///     Defaults to None(lavender)<br />
    ///     The color of the item
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor Color { get; set; }

    /// <summary>
    ///     Default null<br />
    ///     If specified, this is the sprite value used to display the item on the character when equipped
    /// </summary>
    public ushort? DisplaySprite { get; set; }

    /// <summary>
    ///     Default null<br />
    ///     If specified, this is the type of equipment this item is
    /// </summary>
    public EquipmentType? EquipmentType { get; set; }

    /// <summary>
    ///     Default null<br />
    ///     If specified, this is the gender required to use this item
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    ///     Whether or not the item can be dyed. This determines whether a color change applied to an item persists through
    ///     serialization and deserialization. Town dyes on pre-99 armor is not dye, theyre completely different sprites.
    /// </summary>
    public bool IsDyeable { get; set; }

    /// <summary>
    ///     Whether or not an instance of the item can be modified. This controls if an item can be affected by meta node
    ///     mutators.
    /// </summary>
    public bool IsModifiable { get; set; }

    /// <summary>
    ///     Defaults to null<br />
    ///     If specified, the max durability of the item
    /// </summary>
    public int? MaxDurability { get; set; }
    /// <summary>
    ///     The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable
    /// </summary>
    public int MaxStacks { get; set; } = 1;
    /// <summary>
    ///     Defaults to null<br />
    ///     If specified, these are the stats this item grants when equipped
    /// </summary>
    public AttributesSchema? Modifiers { get; set; }

    /// <summary>
    ///     Default null<br />
    ///     If specified, this armor will have pants, and they will be this color
    /// </summary>
    public DisplayColor? PantsColor { get; set; }

    /// <summary>
    ///     The amount of gold given for selling this item to a merchant
    /// </summary>
    public int SellValue { get; set; }

    /// <summary>
    ///     The weight of the item in the inventory, or equipped
    /// </summary>
    public byte Weight { get; set; }
}