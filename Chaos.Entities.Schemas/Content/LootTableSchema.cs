namespace Chaos.Entities.Schemas.Content;

public record LootTableSchema
{
    /// <summary>
    ///     A unique id specific to this loot table. Best practice is to match the file name
    /// </summary>
    public required string Key { get; init; }
    /// <summary>
    ///     A collection of lootDrops. Every item in the list is calculated, allowing multiple drops
    /// </summary>
    public required ICollection<LootDropSchema> LootDrops { get; init; } = Array.Empty<LootDropSchema>();
}