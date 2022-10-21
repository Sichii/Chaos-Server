using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Aisling;

public sealed record MapInstanceSchema
{
    /// <summary>
    ///     A flag, or combination of flags that should affect the map<br />You can combine multiple flags by separating them with commas<br />Ex.
    ///     "Snow, NoTabMap"
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MapFlags Flags { get; init; }

    /// <summary>
    ///     A unique id specific to this map instance<br />Best practice is to match the folder name
    /// </summary>
    [JsonRequired]
    public string InstanceId { get; init; } = null!;

    /// <summary>
    ///     Default null<br />If specified, sets the minimum level needed to enter this map via warp tile
    /// </summary>
    public int? MaximumLevel { get; init; }

    /// <summary>
    ///     Default null<br />If specified, sets the maximum level allowed to enter this map via warp tile
    /// </summary>
    public int? MinimumLevel { get; init; }

    /// <summary>
    ///     The byte values of the music track to play when entering the map<br />These values aren't explored yet, so you'll have to figure out
    ///     what's available yourself
    /// </summary>
    public byte Music { get; init; }

    /// <summary>
    ///     The name of the map that will display in-game
    /// </summary>
    [JsonRequired]
    public string Name { get; init; } = null!;

    /// <summary>
    ///     A collection of script keys to load for this map (TODO: scripts section)
    /// </summary>
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     A string representation of the map id. Ex. 500 for mileth
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;

    /// <summary>
    ///     A collection of warps
    /// </summary>
    public ICollection<WarpSchema> Warps { get; init; } = Array.Empty<WarpSchema>();

    /// <summary>
    ///     A collection of warps that take you to the world map
    /// </summary>
    public ICollection<WorldMapWarpSchema> WorldMapWarps { get; init; } = Array.Empty<WorldMapWarpSchema>();
}