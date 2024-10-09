using Chaos.DarkAges.Definitions;

namespace Chaos.Site.Models;

public sealed class MonsterDto
{
    public int AbilityLevel { get; set; }
    public int Ac { get; set; }
    public int AggroRange { get; set; }
    public int AssailIntervalMs { get; set; }
    public int AtkSpeedPct { get; set; }
    public int Con { get; set; }
    public int Dex { get; set; }
    public int Dmg { get; set; }
    public required string Drops { get; set; }
    public int ExpReward { get; set; }
    public int FlatSkillDamage { get; set; }
    public int FlatSpellDamage { get; set; }
    public int Hit { get; set; }
    public int Int { get; set; }
    public int Level { get; set; }
    public int MagicResistance { get; set; }
    public int MaxGoldDrop { get; set; }
    public int MaximumHp { get; set; }
    public int MaximumMp { get; set; }
    public int MinGoldDrop { get; set; }
    public int MoveIntervalMs { get; set; }
    public required string Name { get; set; }
    public int SkillDamagePct { get; set; }
    public int SkillIntervalMs { get; set; }
    public required string Skills { get; set; }
    public int SpellDamagePct { get; set; }
    public int SpellIntervalMs { get; set; }
    public required string Spells { get; set; }
    public int Sprite { get; set; }
    public int Str { get; set; }
    public CreatureType Type { get; set; }
    public int WanderIntervalMs { get; set; }
    public int Wis { get; set; }
}