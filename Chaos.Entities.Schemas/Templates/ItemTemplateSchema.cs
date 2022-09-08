using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Aisling;

namespace Chaos.Entities.Schemas.Templates;

public record ItemTemplateSchema : PanelObjectTemplateSchema
{
    public required bool AccountBound { get; init; }
    public required AdvClass? AdvClass { get; init; }
    public required BaseClass? BaseClass { get; init; }
    public required DisplayColor Color { get; init; }
    public required ushort? DisplaySprite { get; init; }
    public required EquipmentType? EquipmentType { get; init; }
    public required Gender? Gender { get; init; }
    public required int? MaxDurability { get; init; }
    public required int MaxStacks { get; init; } = 1;
    public required AttributesSchema? Modifiers { get; init; }
    public required int Value { get; init; }
    public required byte Weight { get; init; }
}