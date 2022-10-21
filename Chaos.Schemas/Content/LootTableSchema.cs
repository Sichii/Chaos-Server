using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

public sealed record LootTableSchema
{
    /// <summary>
    ///     A unique id specific to this loot table. Best practice is to match the file name
    /// </summary>
    [JsonRequired]
    public string Key { get; init; } = null!;
    /// <summary>
    ///     A collection of lootDrops. Every item in the list is calculated, allowing multiple drops
    /// </summary>
    public ICollection<LootDropSchema> LootDrops { get; init; } = Array.Empty<LootDropSchema>();
}