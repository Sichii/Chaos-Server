#region
using System.Threading.Channels;
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Services.Other;

/// <summary>
///     Coordinates all map-to-map movement and integrates with the sharding system to prevent race conditions during shard
///     assignment. Traversals to the same destination map are serialized so that sharding decisions always see the result
///     of prior placements
/// </summary>
public sealed class MapTraversalService : BackgroundService, IMapTraversalService
{
    private readonly Channel<TraversalRequest> Channel;
    private readonly ILogger<MapTraversalService> Logger;

    /// <summary>
    ///     Per-destination-map semaphores that serialize sharding decisions and placement for the same base map. The semaphore
    ///     is held through the entire resolve-and-place sequence so that the next aisling's sharding decision always sees
    ///     fully up-to-date MapInstance values. Different destination maps process concurrently
    /// </summary>
    private readonly ConcurrentDictionary<string, SemaphoreSlim> PerMapSemaphores = new(StringComparer.OrdinalIgnoreCase);

    private readonly IShardGenerator ShardGenerator;
    private readonly ISimpleCache SimpleCache;

    public MapTraversalService(IShardGenerator shardGenerator, ISimpleCache simpleCache, ILogger<MapTraversalService> logger)
    {
        ShardGenerator = shardGenerator;
        SimpleCache = simpleCache;
        Logger = logger;

        Channel = System.Threading.Channels.Channel.CreateUnbounded<TraversalRequest>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            });
    }

    /// <inheritdoc />
    public void AdminTraverseMap(Aisling aisling, MapInstance destinationMap, IPoint destinationPoint)
        => Channel.Writer.TryWrite(
            new TraversalRequest(
                aisling,
                destinationMap,
                destinationPoint,
                true,
                false,
                true,
                null));

    /// <inheritdoc />
    public void HandleShardLimiters(MapInstance mapInstance)
    {
        if (mapInstance.ShardingOptions is null)
            return;

        //clean up limiter timers for aislings no longer on this map
        foreach (var aisling in mapInstance.ShardLimiterTimers.Keys)
            if (!mapInstance.TryGetEntity<Aisling>(aisling.Id, out _))
                mapInstance.ShardLimiterTimers.Remove(aisling, out _);

        var limit = mapInstance.ShardingOptions.Limit;

        switch (mapInstance.ShardingOptions.ShardingType)
        {
            case ShardingType.None:
            case ShardingType.PlayerLimit:
            case ShardingType.AlwaysShardOnCreate:
                break;
            case ShardingType.AbsolutePlayerLimit:
            {
                using var rentedAislings = mapInstance.GetEntities<Aisling>()
                                                      .Where(aisling => !aisling.IsAdmin)
                                                      .ToRented();

                var aislings = rentedAislings.Array;

                var amountOverLimit = aislings.Count - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                using var rentedAislingsToRemove = aislings.OrderByDescending(a => a.Id)
                                                           .ThenBy(a =>
                                                           {
                                                               if (a.Group == null)
                                                                   return 0;

                                                               return a.Group.Count(m => m.MapInstance == mapInstance);
                                                           })
                                                           .Take(amountOverLimit)
                                                           .ToRented();

                var aislingsToRemove = rentedAislingsToRemove.Array;

                HandleLimiterTimersAndTraversal(mapInstance, aislingsToRemove, "player");

                break;
            }
            case ShardingType.AbsoluteGroupLimit:
            {
                var aislings = mapInstance.GetEntities<Aisling>()
                                          .Where(aisling => !aisling.IsAdmin);

                //number of unique groups in the zone
                using var rentedGroups = aislings.GroupBy(a => a.Group)
                                                 .SelectMany(grp =>
                                                 {
                                                     if (grp.Key == null)
                                                         return grp.Select(m => new[]
                                                         {
                                                             m
                                                         });

                                                     return
                                                     [
                                                         grp.Where(m => m.MapInstance == mapInstance)
                                                            .ToArray()
                                                     ];
                                                 })
                                                 .ToRented();

                var groups = rentedGroups.Array;
                var groupCount = groups.Count;

                var amountOverLimit = groupCount - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                using var rentedAislingsToRemove = groups.OrderByDescending(grp => grp.Max(a => a.Id))
                                                         .ThenBy(grp => grp.Length)
                                                         .Take(amountOverLimit)
                                                         .SelectMany(a => a)
                                                         .ToRented();

                var aislingsToRemove = rentedAislingsToRemove.Array;

                HandleLimiterTimersAndTraversal(mapInstance, aislingsToRemove, "group");

                break;
            }
            case ShardingType.AbsoluteGuildLimit:
            {
                var aislings = mapInstance.GetEntities<Aisling>()
                                          .Where(aisling => !aisling.IsAdmin);

                //number of unique guilds in the zone
                using var rentedGuilds = aislings.GroupBy(a => a.Guild)
                                                 .SelectMany(gld =>
                                                 {
                                                     if (gld.Key == null)
                                                         return gld.Select(m => new[]
                                                         {
                                                             m
                                                         });

                                                     return
                                                     [
                                                         gld.Where(m => m.MapInstance == mapInstance)
                                                            .ToArray()
                                                     ];
                                                 })
                                                 .ToRented();

                var guilds = rentedGuilds.Array;
                var guildCount = guilds.Count;

                var amountOverLimit = guildCount - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                using var rentedAislingsToRemove = guilds.OrderByDescending(gld => gld.Max(a => a.Id))
                                                         .ThenBy(gld => gld.Length)
                                                         .Take(amountOverLimit)
                                                         .SelectMany(l => l)
                                                         .ToRented();

                var aislingsToRemove = rentedAislingsToRemove.Array;

                HandleLimiterTimersAndTraversal(mapInstance, aislingsToRemove, "guild");

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc />
    public void TraverseMap(
        Creature creature,
        MapInstance destinationMap,
        IPoint destinationPoint,
        bool ignoreSharding = false,
        bool fromWorldMap = false,
        Func<Task>? onTraverse = null)
        => Channel.Writer.TryWrite(
            new TraversalRequest(
                creature,
                destinationMap,
                destinationPoint,
                ignoreSharding,
                fromWorldMap,
                false,
                onTraverse));

    internal MapInstance CreateNewShard(MapInstance baseMap)
    {
        var shard = ShardGenerator.CreateShardOfInstance(baseMap.InstanceId);
        shard.Shards = baseMap.Shards;
        baseMap.Shards.TryAdd(shard.InstanceId, shard);

        return shard;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Channel.Reader
                             .WaitToReadAsync(stoppingToken)
                             .ConfigureAwait(false);

                while (Channel.Reader.TryRead(out var request))
                    _ = ProcessTraversalAsync(request);
            } catch (OperationCanceledException)
            {
                break;
            } catch (Exception e)
            {
                Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Processing)
                      .LogError(e, "Exception in map traversal processing loop");
            }
    }

    private MapInstance? FindGroupMemberShard(Aisling aisling, MapInstance baseMap)
        => aisling.Group
                  ?.Where(a => !a.Equals(aisling))
                  .Select(m => m.MapInstance)
                  .FirstOrDefault(m
                      => m.InstanceId.EqualsI(baseMap.InstanceId) || (m.IsShard && m.BaseInstanceId!.EqualsI(baseMap.InstanceId)));

    private MapInstance? FindGuildMemberShard(Aisling aisling, MapInstance baseMap)
        => aisling.Guild
                  ?.GetOnlineMembers()
                  .Where(a => !a.Equals(aisling))
                  .Select(m => m.MapInstance)
                  .FirstOrDefault(m
                      => m.InstanceId.EqualsI(baseMap.InstanceId) || (m.IsShard && m.BaseInstanceId!.EqualsI(baseMap.InstanceId)));

    private static MapInstance? FindInstanceUnderLimit(MapInstance baseMap, Func<MapInstance, int> countSelector)
    {
        if (countSelector(baseMap) < baseMap.ShardingOptions!.Limit)
            return baseMap;

        foreach (var instance in baseMap.Shards.Values)
            if (countSelector(instance) < baseMap.ShardingOptions.Limit)
                return instance;

        return null;
    }

    private void HandleLimiterTimersAndTraversal(MapInstance mapInstance, ArraySegment<Aisling> aislingsToRemove, string limitType)
    {
        //for each timer that isnt for one of these aislings
        //remove that timer
        foreach (var aislingToRemove in mapInstance.ShardLimiterTimers.Keys.Except(aislingsToRemove))
            mapInstance.ShardLimiterTimers.Remove(aislingToRemove, out _);

        //for each aisling that doesnt have a timer
        //create a timer and send an initial warning
        foreach (var newAisling in aislingsToRemove.Except(mapInstance.ShardLimiterTimers.Keys))
        {
            mapInstance.ShardLimiterTimers.TryAdd(
                newAisling,
                new PeriodicMessageTimer(
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(1),
                    "You will be removed from the map in {Time}",
                    message => newAisling.SendActiveMessage(message)));

            newAisling.SendActiveMessage($"The map has reached it's {limitType} limit");
            newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
        }

        //for each timer that has expired
        //move the player to the exit and remove the timer
        foreach (var kvp in mapInstance.ShardLimiterTimers
                                       .IntersectBy(aislingsToRemove, kvp => kvp.Key)
                                       .ToArray())
            if (kvp.Value.IntervalElapsed)
            {
                var exitMapInstance = SimpleCache.Get<MapInstance>(mapInstance.ShardingOptions!.ExitLocation.Map);
                TraverseMap(kvp.Key, exitMapInstance, mapInstance.ShardingOptions.ExitLocation);
                mapInstance.ShardLimiterTimers.Remove(kvp.Key, out _);
            }
    }

    private async Task ProcessAdminTraversalAsync(TraversalRequest request)
    {
        var aisling = (Aisling)request.Creature;
        var currentMap = aisling.MapInstance;
        var destinationMap = request.DestinationMap;
        var sourcePoint = new Point(aisling.X, aisling.Y);

        await aisling.Client.ReceiveSync.WaitAsync();

        try
        {
            await destinationMap.Initialization;

            var removed = false;
            await currentMap.InvokeAsync(() => removed = currentMap.RemoveEntity(aisling));

            if (!removed)
            {
                aisling.SendActiveMessage("Traversal failed.");

                return;
            }

            if (currentMap.InstanceId != destinationMap.InstanceId)
            {
                aisling.Trackers.LastMapInstance = currentMap;
                aisling.Trackers.LastMapInstanceId = currentMap.InstanceId;
                aisling.Trackers.LastBaseMapInstanceId = currentMap.LoadedFromInstanceId;
            }

            try
            {
                await destinationMap.InvokeAsync(() => destinationMap.AddAislingDirect(aisling, request.DestinationPoint));
            } catch (Exception e)
            {
                Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                      .WithProperty(aisling)
                      .WithProperty(currentMap)
                      .WithProperty(destinationMap)
                      .LogError(
                          e,
                          "Exception thrown while adding aisling {@AislingName} to destination map {@ToMapInstanceId} during admin traverse, attempting recovery",
                          aisling.Name,
                          destinationMap.InstanceId);

                await RecoverFromFailedTraversal(aisling, currentMap, sourcePoint);
            }
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                  .WithProperty(aisling)
                  .WithProperty(currentMap)
                  .WithProperty(destinationMap)
                  .LogError(
                      e,
                      "Exception thrown while creature {@CreatureName} attempted to admin-traverse from map {@FromMapInstanceId} to map {@ToMapInstanceId}",
                      aisling.Name,
                      currentMap.InstanceId,
                      destinationMap.InstanceId);
        } finally
        {
            aisling.Client.ReceiveSync.Release();
        }
    }

    private async Task ProcessStandardTraversalAsync(TraversalRequest request)
    {
        var creature = request.Creature;
        var currentMap = creature.MapInstance;
        var destinationMap = request.DestinationMap;
        var aisling = creature as Aisling;
        var sourcePoint = new Point(creature.X, creature.Y);

        if (aisling is not null)
            await aisling.Client.ReceiveSync.WaitAsync();

        try
        {
            await destinationMap.Initialization;

            //if it's from the world map, remove should be true
            var removed = request.FromWorldMap;

            //if its not from the world map, make sure we successfully remove it
            if (!request.FromWorldMap)
                await currentMap.InvokeAsync(() => removed = currentMap.RemoveEntity(creature));

            if (!removed)
                return;

            //set tracker info
            if (currentMap.InstanceId != destinationMap.InstanceId)
            {
                creature.Trackers.LastMapInstance = currentMap;
                creature.Trackers.LastMapInstanceId = currentMap.InstanceId;
                creature.Trackers.LastBaseMapInstanceId = currentMap.LoadedFromInstanceId;
            }

            var shouldShard = aisling is not null
                              && !request.IgnoreSharding
                              && !destinationMap.IsShard
                              && destinationMap.ShardingOptions is
                              {
                                  ShardingType: not ShardingType.None and not ShardingType.AlwaysShardOnCreate
                              };

            if (shouldShard)
            {
                //acquire per-map semaphore to serialize sharding decisions and placement for this destination.
                //the semaphore is held through AddAislingDirect so that the next aisling's sharding decision
                //sees fully up-to-date MapInstance values on group/guild members
                var semaphore = PerMapSemaphores.GetOrAdd(destinationMap.LoadedFromInstanceId, _ => new SemaphoreSlim(1, 1));

                MapInstance? targetMap = null;

                await semaphore.WaitAsync();

                try
                {
                    targetMap = ResolveShardTarget(aisling!, destinationMap);

                    await targetMap.Initialization;

                    await targetMap.InvokeAsync(() =>
                    {
                        targetMap.AddAislingDirect(aisling!, request.DestinationPoint);
                        request.OnTraverse?.Invoke();
                    });
                } catch (Exception e)
                {
                    Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                          .WithProperty(creature)
                          .WithProperty(currentMap)
                          .WithProperty(targetMap ?? (object)destinationMap)
                          .LogError(
                              e,
                              "Exception thrown while adding aisling {@AislingName} to shard {@ToMapInstanceId}, attempting recovery",
                              aisling!.Name,
                              targetMap?.InstanceId ?? destinationMap.InstanceId);

                    if (!request.FromWorldMap)
                        await RecoverFromFailedTraversal(aisling, currentMap, sourcePoint);
                } finally
                {
                    semaphore.Release();
                }
            } else
                try
                {
                    await destinationMap.InvokeAsync(() =>
                    {
                        if (aisling is not null)
                            destinationMap.AddAislingDirect(aisling, request.DestinationPoint);
                        else
                            destinationMap.AddEntity(creature, request.DestinationPoint);

                        request.OnTraverse?.Invoke();
                    });
                } catch (Exception e)
                {
                    Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                          .WithProperty(creature)
                          .WithProperty(currentMap)
                          .WithProperty(destinationMap)
                          .LogError(
                              e,
                              "Exception thrown while adding creature {@CreatureName} to map {@ToMapInstanceId}, attempting recovery",
                              creature.Name,
                              destinationMap.InstanceId);

                    if (!request.FromWorldMap)
                        await RecoverFromFailedTraversal(creature, currentMap, sourcePoint);
                }
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                  .WithProperty(creature)
                  .WithProperty(currentMap)
                  .WithProperty(destinationMap)
                  .LogError(
                      e,
                      "Exception thrown while creature {@CreatureName} attempted to traverse from map {@FromMapInstanceId} to map {@ToMapInstanceId}",
                      creature.Name,
                      currentMap.InstanceId,
                      destinationMap.InstanceId);
        } finally
        {
            aisling?.Client.ReceiveSync.Release();
        }
    }

    private Task ProcessTraversalAsync(TraversalRequest request)
        => request.IsAdmin ? ProcessAdminTraversalAsync(request) : ProcessStandardTraversalAsync(request);

    private async Task RecoverFromFailedTraversal(Creature creature, MapInstance sourceMap, Point sourcePoint)
    {
        try
        {
            await sourceMap.InvokeAsync(() =>
            {
                if (creature is Aisling aisling)
                    sourceMap.AddAislingDirect(aisling, sourcePoint);
                else
                    sourceMap.AddEntity(creature, sourcePoint);
            });

            if (creature is Aisling recoveredAisling)
                recoveredAisling.SendActiveMessage("Map traversal failed. You have been returned.");
        } catch (Exception recoveryEx)
        {
            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                  .WithProperty(creature)
                  .WithProperty(sourceMap)
                  .LogError(
                      recoveryEx,
                      "Failed to recover creature {@CreatureName} to source map {@MapInstanceId} after failed traversal. Player will need to relog",
                      creature.Name,
                      sourceMap.InstanceId);
        }
    }

    /// <summary>
    ///     Determines which map instance (base map, existing shard, or new shard) to place an aisling on based on the sharding
    ///     rules. Does not add the entity — only resolves the target. Uses EffectiveAislingCount (actual + pending
    ///     reservations) for capacity checks so that sequential sharding decisions under the per-map semaphore see pending
    ///     placements from prior decisions
    /// </summary>
    internal MapInstance ResolveShardTarget(Aisling aisling, MapInstance baseMap)
    {
        switch (baseMap.ShardingOptions!.ShardingType)
        {
            case ShardingType.PlayerLimit:
            case ShardingType.AbsolutePlayerLimit:
            {
                //if the limit is 1, do not re-use instances
                if (baseMap.ShardingOptions.Limit == 1)
                    return CreateNewShard(baseMap);

                //non-absolute player limit will allow group members to go over the normal player limit
                if (baseMap.ShardingOptions.ShardingType == ShardingType.PlayerLimit)
                {
                    var shard = FindGroupMemberShard(aisling, baseMap);

                    if (shard != null)
                        return shard;
                }

                //if the limit is not 1, using previously opened shards is allowed
                //starting with this base instance, check each shard to see if there is an available spot
                if (baseMap.EffectiveAislingCount < baseMap.ShardingOptions.Limit)
                    return baseMap;

                foreach (var instance in baseMap.Shards.Values)
                    if (instance.EffectiveAislingCount < baseMap.ShardingOptions.Limit)
                        return instance;

                //if there is no available spots, create a new shard
                return CreateNewShard(baseMap);
            }
            case ShardingType.AbsoluteGroupLimit:
            {
                //if any group member is already in this instance or a shard of this instance
                var shard = FindGroupMemberShard(aisling, baseMap);

                if (shard != null)
                    return shard;

                //if the limit is 1, do not re-use instances
                if (baseMap.ShardingOptions.Limit == 1)
                    return CreateNewShard(baseMap);

                var target = FindInstanceUnderLimit(
                    baseMap,
                    instance => instance.GetEntities<Aisling>()
                                        .GroupBy(a => a.Group)
                                        .Sum(grp => grp.Key == null ? grp.Count() : 1));

                return target ?? CreateNewShard(baseMap);
            }
            case ShardingType.AbsoluteGuildLimit:
            {
                //if any guild member is already in this instance or a shard of this instance
                var shard = FindGuildMemberShard(aisling, baseMap);

                if (shard != null)
                    return shard;

                //if the limit is 1, do not re-use instances
                if (baseMap.ShardingOptions.Limit == 1)
                    return CreateNewShard(baseMap);

                var target = FindInstanceUnderLimit(
                    baseMap,
                    instance => instance.GetEntities<Aisling>()
                                        .GroupBy(a => a.Guild)
                                        .Sum(gld => gld.Key == null ? gld.Count() : 1));

                return target ?? CreateNewShard(baseMap);
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private sealed record TraversalRequest(
        Creature Creature,
        MapInstance DestinationMap,
        IPoint DestinationPoint,
        bool IgnoreSharding,
        bool FromWorldMap,
        bool IsAdmin,
        Func<Task>? OnTraverse);
}