#region
using Chaos.DarkAges.Definitions;
#endregion

namespace Chaos.Site.Models;

public sealed class ItemDto
{
    public int? AbilityLevel { get; set; }
    public int? Ac { get; set; }
    public bool AccountBound { get; set; }
    public AdvClass? AdvClass { get; set; }
    public int? AtkSpeedPct { get; set; }
    public int? BuyCost { get; set; }
    public required string Category { get; set; }
    public BaseClass? Class { get; set; }
    public DisplayColor? Color { get; set; }
    public int? Con { get; set; }
    public TimeSpan? Cooldown { get; set; }
    public required string Description { get; set; }
    public int? Dex { get; set; }
    public int? Dmg { get; set; }
    public EquipmentType? EquipmentType { get; set; }
    public int? FlatSkillDamage { get; set; }
    public int? FlatSpellDamage { get; set; }
    public Gender? Gender { get; set; }
    public int? Hit { get; set; }
    public int? Hp { get; set; }
    public int? Int { get; set; }
    public bool IsDyeable { get; set; }
    public bool IsModifiable { get; set; }
    public int? Level { get; set; }
    public int? MagicResistance { get; set; }
    public int? MaxDurability { get; set; }
    public int? MaxStacks { get; set; }
    public int? Mp { get; set; }
    public required string Name { get; set; }
    public bool NoTrade { get; set; }
    public DisplayColor? PantsColor { get; set; }
    public bool RequiresMaster { get; set; }
    public int? SellValue { get; set; }
    public int? SkillDamagePct { get; set; }
    public int? SpellDamagePct { get; set; }
    public int? Sprite { get; set; }
    public int? Str { get; set; }
    public string TemplateKey { get; set; } = null!;
    public int? Weight { get; set; }
    public int? Wis { get; set; }
}