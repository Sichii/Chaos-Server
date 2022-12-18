using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record ShardingOptionsSchema
{
    /// <summary>
    ///     The instanceId to teleport players to if they log on and the instance no longer exists
    /// </summary>
    /// <remarks>
    ///     Imagine a scenario where a players disconnects. They should be able to log back into the instance they were already in. <br />
    ///     Now imagine a scenario where a player logs off, then logs back on 5 hours later. The instance will likely no longer exist. <br />
    ///     We can't simply create a new instance for them, this would be abusable (bosses would respawn, events would reset, etc) <br />
    ///     This instanceid is the place to put people who are in this scenario, generally speaking it should be the map you enter this instance
    ///     from
    /// </remarks>
    [JsonRequired]
    public Location ExitLocation { get; init; }

    /// <summary>
    ///     The number of players or groups allowed per instance (based on Shardingtype)
    /// </summary>
    public int Limit { get; init; }
    /// <summary>
    ///     The conditions that lead to new shards of this instance being created
    /// </summary>
    public ShardingType ShardingType { get; init; } = ShardingType.None;
}