using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Content;

public sealed record MapInstanceSchema
{
    /// <summary>
    ///     A flag, or combination of flags that should affect the map<br />You can combine multiple flags by separating them with commas<br />Ex.
    ///     "Snow, NoTabMap"
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MapFlags Flags { get; set; }

    /// <summary>
    ///     A unique id specific to this map instance<br />This must match the name of the folder containing this file
    /// </summary>
    [JsonRequired]
    public string InstanceId { get; set; } = null!;

    /// <summary>
    ///     Default null<br />If specified, sets the minimum level needed to enter this map via warp tile
    /// </summary>
    public int? MaximumLevel { get; set; }

    /// <summary>
    ///     Default null<br />If specified, sets the maximum level allowed to enter this map via warp tile
    /// </summary>
    public int? MinimumLevel { get; set; }

    /// <summary>
    ///     The byte values of the music track to play when entering the map<br />These values aren't explored yet, so you'll have to figure out
    ///     what's available yourself
    /// </summary>
    public byte Music { get; set; }

    /// <summary>
    ///     The name of the map that will display in-game
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     A collection of script keys for scripts to load for this map (TODO: scripts section)
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     Default null<br />
    ///     If specified, these options will be used to determine how this instance will shard itself
    /// </summary>
    public ShardingOptionsSchema? ShardingOptions { get; set; }

    /// <summary>
    ///     A string representation of the map id. Ex. 500 for mileth
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}