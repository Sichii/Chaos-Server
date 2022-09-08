namespace Chaos.Entities.Schemas.Content;

public record LootTableSchema
{
    public required string Key { get; init; }
    public required ICollection<LootDropSchema> LootDrops { get; init; } = Array.Empty<LootDropSchema>();
}