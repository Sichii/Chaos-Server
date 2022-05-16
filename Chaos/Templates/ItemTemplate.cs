using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class ItemTemplate : PanelObjectTemplateBase
{
    public bool AccountBound { get; init; }
    public AdvClass AdvClass { get; init; }
    public BaseClass BaseClass { get; init; }
    public int Cost { get; init; }
    public DisplayColor DefaultColor { get; init; }
    public EquipmentType? EquipmentType { get; init; }
    public Gender Gender { get; init; } = Gender.Unisex;
    public ItemSprite ItemSprite { get; init; } = new(0, 0);
    public int? MaxDurability { get; init; }
    public int MaxStacks { get; init; } = 1;
    public Attributes? Modifiers { get; init; }
    public override ushort PanelSprite => ItemSprite.PanelSprite;
    public bool Stackable { get; init; }
    public override string TemplateKey { get; init; } = "PLACEHOLDER";
    public byte Weight { get; init; }
}