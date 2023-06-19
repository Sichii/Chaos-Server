using System.Text.Json.Serialization;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a monster spawn
/// </summary>
public sealed record MonsterSpawnSchema
{
    /// <summary>
    ///     A collection of points that monsters created by this spawn will not spawn or wander on
    /// </summary>
    public ICollection<Point> BlackList { get; set; } = Array.Empty<Point>();

    /// <summary>
    ///     Default to null, causing monsters to spawn facing random directions<br />If specified, will spawn monsters facing
    ///     in the specified direction
    /// </summary>
    public Direction? Direction { get; set; }

    /// <summary>
    ///     A collection of extra monster script keys to add to the monsters created by this spawn
    /// </summary>
    public ICollection<string> ExtraScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The number of seconds between each trigger of this spawn
    /// </summary>
    public int IntervalSecs { get; set; }

    /// <summary>
    ///     Defaults to 0<br />If specified, will randomize the interval by the percentage specified<br />Ex. With an interval of 60, and a
    ///     Variance of 50, the spawn interval would vary from 45-75secs
    /// </summary>
    public int? IntervalVariancePct { get; set; }

    /// <summary>
    ///     Default is to not have a loot table.<br />
    ///     If specified, the unique id for the loot table used to determine monster drops from this spawn
    /// </summary>
    public string? LootTableKey { get; set; }

    /// <summary>
    ///     The maximum number of monsters that can be on the map from this spawn
    /// </summary>
    public int MaxAmount { get; set; }

    /// <summary>
    ///     The maximum number of monsters to create per interval of this spawn
    /// </summary>
    public int MaxPerSpawn { get; set; }

    /// <summary>
    ///     The unique id for the template of the monster to spawn
    /// </summary>
    [JsonRequired]
    public string MonsterTemplateKey { get; set; } = null!;

    /// <summary>
    ///     Defaults to spawn on entire map<br />If specified, monsters will only spawn within the specified bounds
    /// </summary>
    public Rectangle? SpawnArea { get; set; }
}