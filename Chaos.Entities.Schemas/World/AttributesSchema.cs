namespace Chaos.Entities.Schemas.World;

public record AttributesSchema
{
    public int Ac { get; init; }
    public int Con { get; init; }
    public int Dex { get; init; }
    public int Dmg { get; init; }
    public int Hit { get; init; }
    public int Int { get; init; }
    public int MagicResistance { get; init; }
    public int MaximumHp { get; init; }
    public int MaximumMp { get; init; }
    public int Str { get; init; }
    public int Wis { get; init; }
}