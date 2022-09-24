using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Aisling;

namespace Chaos.Entities.Schemas.Templates;

public record ItemTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     If the item is account bound, it cannot be traded or dropped
    /// </summary>
    public required bool AccountBound { get; init; }

    /// <summary>
    ///     Defaults to None(lavender)<br />If the item is dyeable, this is the dye color
    /// </summary>
    public required DisplayColor Color { get; init; }

    /// <summary>
    ///     Default null<br />If specified, this is the sprite value used to display the item on the character when equipped
    /// </summary>
    public required ushort? DisplaySprite { get; init; }

    /// <summary>
    ///     Defaults to null<br />If specified, the base max durability of the item
    /// </summary>
    public required int? MaxDurability { get; init; }
    /// <summary>
    ///     The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable
    /// </summary>
    public required int MaxStacks { get; init; } = 1;
    /// <summary>
    ///     Defaults to null<br />If specified, these are the stats this item grants when equipped
    /// </summary>
    public required AttributesSchema? Modifiers { get; init; }

    /// <summary>
    ///     Default null<br />If specified, this armor will have pants, and they will be this color
    /// </summary>
    public required DisplayColor? PantsColor { get; init; }

    /// <summary>
    ///     Not fully implemented
    /// </summary>
    public required int Value { get; init; }

    /// <summary>
    ///     The weight of the item in the inventory, or equipped
    /// </summary>
    public required byte Weight { get; init; }
}