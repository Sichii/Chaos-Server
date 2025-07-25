#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates.Abstractions;
#endregion

namespace Chaos.Models.Templates;

public sealed record ItemTemplate : PanelEntityTemplateBase
{
    public required bool AccountBound { get; init; }

    /// <summary>
    ///     When this item is equipped, it will be considered an overcoat for display purposes. This allows items in the normal
    ///     armor slot to use overcoat sprites and be dyeable. If an overcoat is equipped, that item will show instead.
    /// </summary>
    public bool ArmorUsesOvercoatSprites { get; set; }

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
    public required bool NoTrade { get; init; }

    /// <summary>
    ///     When this item is equipped, it will be considered an armor for display purposes. This allows items in the overcoat
    ///     slot to use normal armor sprites. If both an armor and an overcoat are equipped, the overcoat will display.
    /// </summary>
    public bool OvercoatUsesArmorSprites { get; set; }

    /// <summary>
    ///     When this item is equipped, the boots sprite will be set to 0
    /// </summary>
    public bool OverridesBootsSprite { get; set; }

    /// <summary>
    ///     When this item is equipped, the head sprite will be set to 0
    /// </summary>
    public bool OverridesHeadSprite { get; set; }

    public override required ushort PanelSprite
    {
        get => ItemSprite.PanelSprite;
        init { }
    }

    public required DisplayColor? PantsColor { get; init; }
    public required bool PreventBanking { get; init; }
    public required int SellValue { get; init; }

    public required byte Weight { get; init; } = 1;
    public bool Stackable => MaxStacks > 1;
}