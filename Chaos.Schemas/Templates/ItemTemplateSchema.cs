using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Aisling;

namespace Chaos.Schemas.Templates;

public sealed record ItemTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     If the item is account bound, it cannot be traded or dropped
    /// </summary>
    public bool AccountBound { get; init; }

    /// <summary>
    ///     The amount of gold it costs to buy this item from a merchant
    /// </summary>
    public int BuyCost { get; init; }

    /// <summary>
    ///     Defaults to None(lavender)<br />If the item is dyeable, this is the dye color
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor Color { get; init; }

    /// <summary>
    ///     Default null<br />If specified, this is the sprite value used to display the item on the character when equipped
    /// </summary>
    public ushort? DisplaySprite { get; init; }

    /// <summary>
    ///     Defaults to null<br />If specified, the base max durability of the item
    /// </summary>
    public int? MaxDurability { get; init; }
    /// <summary>
    ///     The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable
    /// </summary>
    public int MaxStacks { get; init; } = 1;
    /// <summary>
    ///     Defaults to null<br />If specified, these are the stats this item grants when equipped
    /// </summary>
    public AttributesSchema? Modifiers { get; init; }

    /// <summary>
    ///     Default null<br />If specified, this armor will have pants, and they will be this color
    /// </summary>
    public DisplayColor? PantsColor { get; init; }

    /// <summary>
    ///     The amount of gold given for selling this item to a merchant
    /// </summary>
    public int SellValue { get; init; }

    /// <summary>
    ///     The weight of the item in the inventory, or equipped
    /// </summary>
    public byte Weight { get; init; }
}