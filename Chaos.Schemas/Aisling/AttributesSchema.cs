namespace Chaos.Schemas.Aisling;

public record AttributesSchema : StatsSchema
{
    public int Ac { get; init; }
    public int AtkSpeedPct { get; init; }
    public int Dmg { get; init; }
    public int Hit { get; init; }
    public int MagicResistance { get; init; }
    public int MaximumHp { get; init; }
    public int MaximumMp { get; init; }
}