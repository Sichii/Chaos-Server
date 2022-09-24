using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Data;

namespace Chaos.Entities.Schemas.Aisling;

public record MapInstanceSchema
{
    /// <summary>
    ///     A flag, or combination of flags that should affect the map<br />You can combine multiple flags by separating them with commas<br />Ex.
    ///     "Snow, NoTabMap"
    /// </summary>
    public required MapFlags Flags { get; init; }

    /// <summary>
    ///     A unique id specific to this map instance<br />Best practice is to match the folder name
    /// </summary>
    public required string InstanceId { get; init; }

    /// <summary>
    ///     Default null<br />If specified, sets the minimum level needed to enter this map via warp tile
    /// </summary>
    public required int? MaximumLevel { get; init; }

    /// <summary>
    ///     Default null<br />If specified, sets the maximum level allowed to enter this map via warp tile
    /// </summary>
    public required int? MinimumLevel { get; init; }

    /// <summary>
    ///     The byte values of the music track to play when entering the map<br />These values aren't explored yet, so you'll have to figure out
    ///     what's available yourself
    /// </summary>
    public required byte Music { get; init; }

    /// <summary>
    ///     The name of the map that will display in-game
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     A collection of script keys to load for this map (TODO: scripts section)
    /// </summary>
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     A string representation of the map id. Ex. 500 for mileth
    /// </summary>
    public required string TemplateKey { get; init; }

    /// <summary>
    ///     A collection of warps
    /// </summary>
    public required WarpSchema[] Warps { get; init; } = Array.Empty<WarpSchema>();
}