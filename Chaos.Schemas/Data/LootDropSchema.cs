using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of a loot drop as part of a loot table
/// </summary>
public sealed record LootDropSchema
{
    /// <summary>
    ///     The chance of the item to drop (MUST be between 0 and 1)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public decimal DropChance { get; set; }

    /// <summary>
    ///     A unique id specific to the template of the item that should drop
    /// </summary>
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;
}