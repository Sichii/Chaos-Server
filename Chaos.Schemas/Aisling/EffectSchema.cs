namespace Chaos.Schemas.Aisling;

public sealed record EffectSchema
{
    public required string Name { get; init; }
    public required int? RemainingSecs { get; init; }
}