using Chaos.Data;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class ItemTemplate : PanelObjectTemplateBase
{
    public bool AccountBound { get; init; }
    public AdvClass? AdvClass { get; init; }
    public BaseClass? BaseClass { get; init; }
    public DisplayColor Color { get; init; }
    public EquipmentType? EquipmentType { get; init; }
    public Gender? Gender { get; init; }
    public ItemSprite ItemSprite { get; init; } = new(0, 0);
    public int? MaxDurability { get; init; }
    public int MaxStacks { get; init; } = 1;
    public Attributes? Modifiers { get; init; }
    public override ushort PanelSprite => ItemSprite.PanelSprite;
    public bool Stackable { get; init; }
    public override string TemplateKey { get; init; } = "PLACEHOLDER";
    public int Value { get; init; }
    public byte Weight { get; init; }
}