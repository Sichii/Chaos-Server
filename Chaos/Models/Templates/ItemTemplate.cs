using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates.Abstractions;

namespace Chaos.Models.Templates;

public sealed record ItemTemplate : PanelEntityTemplateBase
{
    public required bool AccountBound { get; init; }

    public required int BuyCost { get; init; }
    public required string Category { get; init; } = string.Empty;
    public required DisplayColor Color { get; init; }
    public required EquipmentType? EquipmentType { get; init; }
    public required Gender? Gender { get; init; }
    public required bool IsDyeable { get; init; }
    public required bool IsModifiable { get; init; }
    public required ItemSprite ItemSprite { get; init; } = null!;
    public required int? MaxDurability { get; init; }
    public required int MaxStacks { get; init; } = 1;
    public required Attributes? Modifiers { get; init; }

    public override required ushort PanelSprite
    {
        get => ItemSprite.PanelSprite;
        init { }
    }

    public required DisplayColor? PantsColor { get; init; }
    public required int SellValue { get; init; }
    public required byte Weight { get; init; } = 1;
    public bool Stackable => MaxStacks > 1;
}