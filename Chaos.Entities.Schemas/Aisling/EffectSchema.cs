namespace Chaos.Entities.Schemas.Aisling;

public record EffectSchema
{
    public required string Name { get; init; }
    public required int? RemainingSecs { get; init; }
}