#region
using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Services.Other.Abstractions;

/// <summary>
///     A service that coordinates all map-to-map movement and integrates with the sharding system
/// </summary>
public interface IMapTraversalService
{
    /// <summary>
    ///     Admin traversal that bypasses sharding. Uses sequential InvokeAsync calls on source and destination maps
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to traverse
    /// </param>
    /// <param name="destinationMap">
    ///     The destination map instance
    /// </param>
    /// <param name="destinationPoint">
    ///     The point on which to place the aisling
    /// </param>
    void AdminTraverseMap(Aisling aisling, MapInstance destinationMap, IPoint destinationPoint);

    /// <summary>
    ///     Handles shard limit enforcement for a map instance. Should be called from the map's update loop when the shard
    ///     limiter timer has elapsed
    /// </summary>
    /// <param name="mapInstance">
    ///     The map instance to enforce shard limits on
    /// </param>
    void HandleShardLimiters(MapInstance mapInstance);

    /// <summary>
    ///     Traverses a creature from its current map to the destination map, respecting sharding rules unless ignoreSharding
    ///     is set
    /// </summary>
    /// <param name="creature">
    ///     The creature to traverse
    /// </param>
    /// <param name="destinationMap">
    ///     The destination map instance. If sharding is enabled, this should be the base map
    /// </param>
    /// <param name="destinationPoint">
    ///     The point on which to place the creature
    /// </param>
    /// <param name="ignoreSharding">
    ///     Whether to bypass sharding logic
    /// </param>
    /// <param name="fromWorldMap">
    ///     Whether the creature is traversing from the world map (skip removal from current map)
    /// </param>
    /// <param name="onTraverse">
    ///     An optional callback to invoke after the creature has been placed on the destination map
    /// </param>
    void TraverseMap(
        Creature creature,
        MapInstance destinationMap,
        IPoint destinationPoint,
        bool ignoreSharding = false,
        bool fromWorldMap = false,
        Func<Task>? onTraverse = null);
}