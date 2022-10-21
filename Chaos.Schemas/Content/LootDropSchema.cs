using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

public sealed record LootDropSchema
{
    /// <summary>
    ///     A unique id specific to the template of the item that should drop
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public int DropChance { get; init; }
    /// <summary>
    ///     The chance of the item to drop
    /// </summary>
    [JsonRequired]
    public string ItemTemplateKey { get; init; } = null!;
}