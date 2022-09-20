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
    ///     Defaults to null<br />If specified, this advanced class flag is required to equip this item
    /// </summary>
    public required AdvClass? AdvClass { get; init; }
    /// <summary>
    ///     Defaults to null<br />If specified, this base class flag is required to equip this item
    /// </summary>
    public required BaseClass? BaseClass { get; init; }
    /// <summary>
    ///     Defaults to None(lavender)<br />If the item is dyeable, this is the dye color
    /// </summary>
    public required DisplayColor Color { get; init; }
    /// <summary>
    ///     Defaults to null<br />If specified, this is the sprite used to display the item on character when it is equipped
    /// </summary>
    public required ushort? DisplaySprite { get; init; }
    /// <summary>
    ///     Default to null<br />If specified, this is type of equipment the item is, determining what slot it can be equipped to
    /// </summary>
    public required EquipmentType? EquipmentType { get; init; }
    /// <summary>
    ///     Defaults to null<br />If specified, player must be of this gender to equip the item
    /// </summary>
    public required Gender? Gender { get; init; }
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
    ///     Not fully implemented
    /// </summary>
    public required int Value { get; init; }
    /// <summary>
    ///     The weight of the item in the inventory, or equipped
    /// </summary>
    public required byte Weight { get; init; }
}