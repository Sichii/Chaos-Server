#region
using System.Text.Json.Serialization;
#endregion

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
    ///     A collection of extra merchant script keys to add to the monsters created by this spawn
    /// </summary>
    public ICollection<string> ExtraScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A unique id specific to the template of the item that should drop
    /// </summary>
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;
}