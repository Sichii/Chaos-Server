namespace Chaos.Entities.Schemas.World;

public record EffectSchema
{
    public string Name { get; init; } = null!;
    public int? RemainingSecs { get; init; }
}