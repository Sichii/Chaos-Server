namespace Chaos.DataObjects;

public record Attributes
{
    public int Ac { get; set; }
    public int AttackSpeed { get; set; }

    public int Con { get; set; }

    public int CooldownReduction { get; set; }

    public int Dex { get; set; }

    public int Dmg { get; set; }

    public int Hit { get; set; }

    public int Int { get; set; }

    public int MagicResistance { get; set; }

    public int MaximumHp { get; set; }

    public int MaximumMp { get; set; }
    public int Str { get; set; }

    public int Wis { get; set; }
}