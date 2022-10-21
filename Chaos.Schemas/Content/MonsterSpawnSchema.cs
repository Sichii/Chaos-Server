using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

public sealed record MonsterSpawnSchema
{
    /// <summary>
    ///     Defaults to 0<br />If specified, monsters created by this spawn will be aggressive and attack enemies if they come within the specified
    ///     distance
    /// </summary>
    public int AggroRange { get; init; } = -1;

    /// <summary>
    ///     The amount of exp monsters created by this spawn will reward when killed
    /// </summary>
    public int ExpReward { get; init; }

    /// <summary>
    ///     A collection of extra monster script keys to add to the monsters created by this spawn
    /// </summary>
    public ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     A number of seconds between each trigger of this spawn
    /// </summary>
    public int IntervalSecs { get; init; }

    /// <summary>
    ///     Defaults to 0<br />If specified, will randomize the interval by the percentage specified<br />Ex. With an interval of 60, and a
    ///     Variance of 50, the spawn interval would var from 45-75secs
    /// </summary>
    public int? IntervalVariancePct { get; init; }

    /// <summary>
    ///     Default is to not have a loot table. If specified, the unique id for the loot table used to determine monster drops from this spawn
    /// </summary>
    public string? LootTableKey { get; init; }

    /// <summary>
    ///     The maximum number of monsters that can be on the map from this spawn
    /// </summary>
    public int MaxAmount { get; init; }

    /// <summary>
    ///     Maximum amount of gold for monsters created by this spawn to drop
    /// </summary>
    public int MaxGoldDrop { get; init; }

    /// <summary>
    ///     The maximum number of monsters to create per interval of this spawn
    /// </summary>
    public int MaxPerSpawn { get; init; }

    /// <summary>
    ///     Minimum amount of gold for monsters created by this spawn to drop
    /// </summary>
    public int MinGoldDrop { get; init; }

    /// <summary>
    ///     The unique id for the template of the monster to spawn
    /// </summary>
    [JsonRequired]
    public string MonsterTemplateKey { get; init; } = null!;

    /// <summary>
    ///     Defaults to spawn on entire map<br />If specified, monsters will only spawn within the specified bounds
    /// </summary>
    public Rectangle? SpawnArea { get; init; }
}