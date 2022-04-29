using System.Text.Json.Serialization;
using Chaos.Core.Definitions;
using Chaos.DataObjects;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class ItemTemplate : PanelObjectTemplateBase
{
    public bool AccountBound { get; init; } = false;
    public AdvClass AdvClass { get; init; } = AdvClass.None;
    public BaseClass BaseClass { get; init; } = BaseClass.Peasant;
    public int Cost { get; init; }
    public DisplayColor DefaultColor { get; init; } = DisplayColor.None;
    public EquipmentSlot EquipmentSlot { get; init; } = EquipmentSlot.None;
    public Gender Gender { get; init; } = Gender.Unisex;
    public ItemSprite ItemSprite { get; init; } = new(0, 0);
    public int? MaxDurability { get; init; }
    public int MaxStacks { get; init; } = 1;
    public Attributes? Modifiers { get; init; }
    public bool Stackable { get; init; } = false;
    public override string TemplateKey { get; init; } = "PLACEHOLDER";
    public byte Weight { get; init; } = 0;
    [JsonIgnore]
    public override ushort Sprite => ItemSprite.Sprite;
}