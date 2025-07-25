#region
using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;
#endregion

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a loot table
/// </summary>
public sealed record LootTableSchema
{
    /// <summary>
    ///     A unique id specific to this loot table. Best practice is to match the file name
    /// </summary>
    [JsonRequired]
    public string Key { get; set; } = null!;

    /// <summary>
    ///     A collection of lootDrops. Every item in the list is calculated, allowing multiple drops
    /// </summary>
    public ICollection<LootDropSchema> LootDrops { get; set; } = [];

    /// <summary>
    ///     The mode in which the loot table is calculated
    /// </summary>
    public LootTableMode Mode { get; set; } = LootTableMode.ChancePerItem;
}