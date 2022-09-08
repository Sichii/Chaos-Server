namespace Chaos.Entities.Schemas.Aisling;

public record AttributesSchema
{
    public required int Ac { get; init; }
    public required int Con { get; init; }
    public required int Dex { get; init; }
    public required int Dmg { get; init; }
    public required int Hit { get; init; }
    public required int Int { get; init; }
    public required int MagicResistance { get; init; }
    public required int MaximumHp { get; init; }
    public required int MaximumMp { get; init; }
    public required int Str { get; init; }
    public required int Wis { get; init; }
}