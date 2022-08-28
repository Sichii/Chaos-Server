using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.World;

namespace Chaos.Entities.Schemas.Templates;

public record ItemTemplateSchema : PanelObjectTemplateSchema
{
    public bool AccountBound { get; init; }
    public AdvClass? AdvClass { get; init; }
    public BaseClass? BaseClass { get; init; }
    public DisplayColor Color { get; init; }
    public ushort? DisplaySprite { get; init; }
    public EquipmentType? EquipmentType { get; init; }
    public Gender? Gender { get; init; }
    public int? MaxDurability { get; init; }
    public int MaxStacks { get; init; } = 1;
    public AttributesSchema? Modifiers { get; init; }
    public int Value { get; init; }
    public byte Weight { get; init; }
}