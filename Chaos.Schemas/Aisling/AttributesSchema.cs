namespace Chaos.Schemas.Aisling;

public record AttributesSchema : StatsSchema
{
    public int Ac { get; set; }
    public int AtkSpeedPct { get; set; }
    public int Dmg { get; set; }
    public int FlatSkillDamage { get; set; }
    public int FlatSpellDamage { get; set; }
    public int Hit { get; set; }
    public int MagicResistance { get; set; }
    public int MaximumHp { get; set; }
    public int MaximumMp { get; set; }
    public int SkillDamagePct { get; set; }
    public int SpellDamagePct { get; set; }
}