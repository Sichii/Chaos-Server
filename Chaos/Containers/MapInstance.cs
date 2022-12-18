using System.Runtime.InteropServices;
using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MapScripts.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Containers;

public sealed class MapInstance : IScripted<IMapScript>, IDeltaUpdatable
{
    private readonly IIntervalTimer HandleShardLimitersTimer;
    private readonly List<MonsterSpawn> MonsterSpawns;
    private readonly MapEntityCollection Objects;
    private readonly IShardGenerator ShardGenerator;
    private readonly ISimpleCache SimpleCache;
    public string? BaseInstanceId { get; set; }
    public MapFlags Flags { get; set; }
    public string InstanceId { get; init; }
    public int? MaximumLevel { get; set; }
    public int? MinimumLevel { get; set; }
    public byte Music { get; set; }
    public string Name { get; set; }
    public IPathfindingService Pathfinder { get; set; } = null!;
    public ShardingOptions? ShardingOptions { get; init; }
    public ConcurrentDictionary<Aisling, IIntervalTimer> ShardLimiterTimers { get; set; }
    public ConcurrentDictionary<string, MapInstance> Shards { get; set; }
    public MapTemplate Template { get; set; }
    public bool IsShard => !string.IsNullOrEmpty(BaseInstanceId);
    /// <inheritdoc />
    public IMapScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }
    public FifoAutoReleasingSemaphoreSlim Sync { get; }

    public MapInstance(
        MapTemplate template,
        ISimpleCache simpleCache,
        IShardGenerator shardGenerator,
        IScriptProvider scriptProvider,
        string name,
        string instanceId,
        ICollection<string>? extraScriptKeys = null
    )
    {
        Name = name;
        InstanceId = instanceId;
        SimpleCache = simpleCache;
        var walkableArea = template.Height * template.Width - template.Tiles.Flatten().Count(t => t.IsWall);
        Objects = new MapEntityCollection(template.Width, template.Height, walkableArea);
        MonsterSpawns = new List<MonsterSpawn>();
        Sync = new FifoAutoReleasingSemaphoreSlim(1, 1);
        Template = template;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        Shards = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);
        ShardLimiterTimers = new ConcurrentDictionary<Aisling, IIntervalTimer>();
        ShardGenerator = shardGenerator;
        HandleShardLimitersTimer = new IntervalTimer(TimeSpan.FromSeconds(10));

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<IMapScript, MapInstance>(ScriptKeys, this);
    }

    public void AddAislingDirect(Aisling aisling, IPoint point) => InnerAddObject(aisling, point);

    public void AddObject(VisibleEntity visibleEntity, IPoint point)
    {
        //shards cant shard, shardtype none means no sharding, non-aisling cant create shards
        if (IsShard
            || (ShardingOptions == null)
            || (ShardingOptions.ShardingType == ShardingType.None)
            || visibleEntity is not Aisling aisling)
            InnerAddObject(visibleEntity, point);
        else
            HandleSharding(aisling, point);
    }

    public void AddObjects<T>(ICollection<T> visibleObjects) where T: VisibleEntity
    {
        if (visibleObjects.Count == 0)
            return;

        foreach (var visibleObj in visibleObjects)
        {
            if (visibleObj is Creature creature)
                Script.OnEntered(creature);

            visibleObj.MapInstance = this;
            Objects.Add(visibleObj.Id, visibleObj);
        }

        foreach (var creature in Objects.Values<Creature>())
        {
            var objectsInRange = visibleObjects
                                 .ThatAreWithinRange(creature)
                                 .ToList();

            foreach (var nearbyCreature in objectsInRange.OfType<Creature>())
                Helpers.HandleApproach(creature, nearbyCreature);

            if (creature is Aisling aisling && objectsInRange.Any())
                aisling.Client.SendVisibleObjects(objectsInRange.ThatAreVisibleTo(aisling));
        }
    }

    public void AddSpawner(MonsterSpawn monsterSpawn)
    {
        monsterSpawn.MapInstance = this;

        monsterSpawn.SpawnArea ??= new Rectangle(
            0,
            0,
            Template.Width,
            Template.Height);

        MonsterSpawns.Add(monsterSpawn);
    }

    private void AddToNewShard(Aisling aisling, IPoint point)
    {
        var shard = ShardGenerator.CreateShardOfInstance(InstanceId);
        shard.Shards = Shards;
        Shards.TryAdd(shard.InstanceId, shard);

        shard.AddAislingDirect(aisling, point);
    }

    public void Click(uint id, Aisling source)
    {
        if (TryGetObject<VisibleEntity>(id, out var obj))
            if (obj.WithinRange(source) && obj.IsVisibleTo(source))
                obj.OnClicked(source);
    }

    public void Click(IPoint point, Aisling source)
    {
        var door = Objects.AtPoint<Door>(point)
                          .TopOrDefault();

        if (door != null)
            door.OnClicked(source);
        else
        {
            var obj = Objects.AtPoint<ReactorTile>(point)
                             .TopOrDefault();

            obj?.OnClicked(source);
        }
    }

    public void Destroy()
    {
        Shards.Remove(InstanceId, out _);
        Objects.Clear();
        MonsterSpawns.Clear();
    }

    public IEnumerable<T> GetEntities<T>() where T: MapEntity => Objects.Values<T>();

    public IEnumerable<T> GetEntitiesAtPoint<T>(IPoint point) where T: MapEntity => Objects.AtPoint<T>(point);

    public IEnumerable<T> GetEntitiesAtPoints<T>(IEnumerable<IPoint> points) where T: MapEntity => Objects.AtPoints<T>(points);

    public IEnumerable<T> GetEntitiesWithinRange<T>(IPoint point, int range = 13) where T: MapEntity =>
        Objects.WithinRange<T>(point, range);

    private void HandleSharding(Aisling aisling, IPoint point)
    {
        switch (ShardingOptions!.ShardingType)
        {
            case ShardingType.None:
                InnerAddObject(aisling, point);

                break;
            case ShardingType.PlayerLimit:
            case ShardingType.AbsolutePlayerLimit:
            {
                //if the limit is 1, do not re-use instances
                if (ShardingOptions.Limit == 1)
                    AddToNewShard(aisling, point);
                else
                {
                    //non-absolute player limit will allow group members to go over the normal player limit
                    if (ShardingOptions.ShardingType == ShardingType.PlayerLimit)
                    {
                        var shard = aisling.Group?
                                           .Where(a => !a.Equals(aisling))
                                           .Select(m => m.MapInstance)
                                           .FirstOrDefault(
                                               m => m.InstanceId.EqualsI(InstanceId)
                                                    || (m.IsShard && m.BaseInstanceId!.EqualsI(InstanceId)));

                        if (shard != null)
                        {
                            //add this player to that instance or shard
                            shard.AddAislingDirect(aisling, point);

                            return;
                        }
                    }

                    //if the limit is not 1, using previously opened shards is allowed
                    //starting with this base instance, check each shard to see if there is an available spot
                    foreach (var instance in Shards.Values.Prepend(this))
                    {
                        var playerCount = instance.Objects.Values<Aisling>().Count();

                        if (playerCount < ShardingOptions.Limit)
                        {
                            instance.InnerAddObject(aisling, point);

                            return;
                        }
                    }

                    //if there is no available spots, create a new shard
                    AddToNewShard(aisling, point);
                }

                break;
            }
            case ShardingType.AbsoluteGroupLimit:
            {
                //if any group member is already in this instance or a shard of this instance
                var shard = aisling.Group?
                                   .Where(a => !a.Equals(aisling))
                                   .Select(m => m.MapInstance)
                                   .FirstOrDefault(
                                       m => m.InstanceId.EqualsI(InstanceId) || (m.IsShard && m.BaseInstanceId!.EqualsI(InstanceId)));

                if (shard != null)
                {
                    //add this player to that instance or shard
                    shard.AddAislingDirect(aisling, point);

                    return;
                }

                if (ShardingOptions.Limit == 1)
                    AddToNewShard(aisling, point);
                else
                {
                    foreach (var instance in Shards.Values.Prepend(this))
                    {
                        //get the number of groups in this instance
                        var groupCount = instance.Objects.Values<Aisling>()
                                                 .GroupBy(a => a.Group)
                                                 .Sum(grp => grp.Key == null ? grp.Count() : 1);

                        //if this instance isnt at the group limit
                        if (groupCount < ShardingOptions.Limit)
                        {
                            //add this player to that instance and return
                            instance.InnerAddObject(aisling, point);

                            return;
                        }
                    }

                    //if we couldnt find a suitable instance to place the player, generate a new one and add them to it
                    AddToNewShard(aisling, point);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleShardLimiters()
    {
        if (ShardingOptions is null)
            return;

        //clean up limiter timers
        foreach (var aisling in ShardLimiterTimers.Keys)
            if (!Objects.ContainsKey(aisling.Id))
                ShardLimiterTimers.Remove(aisling, out _);

        var limit = ShardingOptions.Limit;

        switch (ShardingOptions.ShardingType)
        {
            case ShardingType.None:
            case ShardingType.PlayerLimit:
                break;
            case ShardingType.AbsolutePlayerLimit:
            {
                var aislings = Objects.Values<Aisling>().ToList();
                var amountOverLimit = aislings.Count - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                var aislingsToRemove = aislings.OrderByDescending(a => a.Id)
                                               .ThenBy(
                                                   a =>
                                                   {
                                                       if (a.Group == null)
                                                           return 0;

                                                       return a.Group.Count(m => m.MapInstance == this);
                                                   })
                                               .Take(amountOverLimit)
                                               .ToHashSet();

                //for each timer that isnt for one of these aislings
                //remove that timer
                foreach (var aislingToRemove in ShardLimiterTimers.Keys.Except(aislingsToRemove))
                    ShardLimiterTimers.Remove(aislingToRemove, out _);

                //for each aisling that doesnt have a timer
                //create a timer and send an initial warning
                foreach (var newAisling in aislingsToRemove.Except(ShardLimiterTimers.Keys))
                {
                    ShardLimiterTimers.TryAdd(newAisling, new ShardExitTimer(newAisling, TimeSpan.FromSeconds(15)));
                    newAisling.SendActiveMessage("The map has reached it's player limit");
                    newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
                }

                //for each timer that has expired
                //move the player to the exit and remove the timer
                foreach (var kvp in ShardLimiterTimers.IntersectBy(aislingsToRemove, kvp => kvp.Key).ToList())
                    if (kvp.Value.IntervalElapsed)
                    {
                        var exitMapInstance = SimpleCache.Get<MapInstance>(ShardingOptions.ExitLocation.Map);
                        kvp.Key.TraverseMap(exitMapInstance, ShardingOptions.ExitLocation);
                        ShardLimiterTimers.Remove(kvp.Key, out _);
                    }

                break;
            }
            case ShardingType.AbsoluteGroupLimit:
            {
                var aislings = Objects.Values<Aisling>().ToList();

                //number of unique groups in the zone
                var groups = aislings.GroupBy(a => a.Group)
                                     .SelectMany(
                                         grp =>
                                         {
                                             if (grp.Key == null)
                                                 return grp.Select(
                                                     m => new List<Aisling>
                                                         { m });

                                             return new[] { grp.Where(m => m.MapInstance == this).ToList() };
                                         })
                                     .ToList();

                var groupCount = groups.Count;

                var amountOverLimit = groupCount - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                var groupsToRemove = groups.OrderByDescending(grp => grp.Max(a => a.Id))
                                           .ThenBy(grp => grp.Count)
                                           .Take(amountOverLimit)
                                           .ToList();

                var aislingsToRemove = groupsToRemove.SelectMany(_ => _)
                                                     .ToList();

                //for each timer that isnt for one of these aislings
                //remove that timer
                foreach (var aislingToRemove in ShardLimiterTimers.Keys.Except(aislingsToRemove))
                    ShardLimiterTimers.Remove(aislingToRemove, out _);

                //for each aisling that doesnt have a timer
                //create a timer and send an initial warning
                foreach (var newAisling in aislingsToRemove.Except(ShardLimiterTimers.Keys))
                {
                    ShardLimiterTimers.TryAdd(newAisling, new ShardExitTimer(newAisling, TimeSpan.FromSeconds(15)));
                    newAisling.SendActiveMessage("The map has reached it's group limit");
                    newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
                }

                //for each timer that has expired
                //move the player to the exit and remove the timer
                foreach (var kvp in ShardLimiterTimers.IntersectBy(aislingsToRemove, kvp => kvp.Key).ToList())
                    if (kvp.Value.IntervalElapsed)
                    {
                        var exitMapInstance = SimpleCache.Get<MapInstance>(ShardingOptions.ExitLocation.Map);
                        kvp.Key.TraverseMap(exitMapInstance, ShardingOptions.ExitLocation);
                        ShardLimiterTimers.Remove(kvp.Key, out _);
                    }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InnerAddObject(VisibleEntity visibleEntity, IPoint point)
    {
        Objects.Add(visibleEntity.Id, visibleEntity);
        visibleEntity.SetLocation(this, point);

        if (visibleEntity is Creature c)
            Script.OnEntered(c);

        if (visibleEntity is Aisling aisling)
        {
            (var aislings, var doors, var otherVisibles) = Objects.WithinRange<VisibleEntity>(point)
                                                                  .PartitionBySendType();

            foreach (var otherCreature in otherVisibles.OfType<Creature>())
                Helpers.HandleApproach(aisling, otherCreature);

            aisling.Client.SendMapChangePending();
            aisling.Client.SendMapInfo();
            aisling.Client.SendLocation();

            foreach (var nearbyUser in aislings)
            {
                if (nearbyUser.Equals(aisling))
                    continue;

                if (aisling.IsVisibleTo(nearbyUser))
                    nearbyUser.Client.SendDisplayAisling(aisling);

                if (nearbyUser.IsVisibleTo(aisling))
                    aisling.Client.SendDisplayAisling(nearbyUser);
            }

            aisling.Client.SendVisibleObjects(otherVisibles);
            aisling.Client.SendDoors(doors);
            aisling.Client.SendMapChangeComplete();
            aisling.Client.SendSound(Music, true);
            aisling.Client.SendMapLoadComplete();
            aisling.Client.SendDisplayAisling(aisling);
        } else
        {
            //fast path for non creatures
            if (visibleEntity is not Creature creature)
            {
                foreach (var nearbyAisling in Objects.WithinRange<Aisling>(visibleEntity)
                                                     .ThatCanSee(visibleEntity))
                    nearbyAisling.Client.SendVisibleObjects(visibleEntity);

                return;
            }

            foreach (var nearbyCreature in Objects.WithinRange<Creature>(creature))
            {
                if (nearbyCreature.Equals(creature))
                    continue;

                Helpers.HandleApproach(creature, nearbyCreature);

                if (nearbyCreature is Aisling nearbyAisling && creature.IsVisibleTo(nearbyCreature))
                    nearbyAisling.Client.SendVisibleObjects(visibleEntity);
            }
        }
    }

    public bool IsWalkable(IPoint point, CreatureType creatureType)
    {
        var creatures = Objects.AtPoint<Creature>(point)
                               .ToList();

        return creatureType switch
        {
            CreatureType.Normal => !IsWallToCreaturesOnly(point)
                                   && !IsWall(point)
                                   && !creatures.Any(c => c.Type.WillCollideWith(creatureType)),
            CreatureType.WalkThrough => !IsWallToCreaturesOnly(point) && !creatures.Any(c => c.Type.WillCollideWith(creatureType)),
            CreatureType.Merchant => !IsWallToCreaturesOnly(point)
                                     && !IsWall(point)
                                     && !creatures.Any(c => c.Type.WillCollideWith(creatureType)),
            CreatureType.WhiteSquare => !IsWallToCreaturesOnly(point)
                                        && !IsWall(point)
                                        && !creatures.Any(c => c.Type.WillCollideWith(creatureType)),
            CreatureType.Aisling => !IsWall(point) && !creatures.Any(c => c.Type.WillCollideWith(creatureType)),
            _                    => throw new ArgumentOutOfRangeException(nameof(creatureType), creatureType, null)
        };
    }

    public bool IsWall(IPoint point)
    {
        if (Template.IsWall(point))
            return true;

        var door = GetEntitiesAtPoint<Door>(point).FirstOrDefault();

        return door?.Closed ?? false;
    }

    public bool IsWallToCreaturesOnly(IPoint point) => Objects.AtPoint<ReactorTile>(point).Any(rt => rt.ShouldBlockPathfinding);

    public bool IsWithinMap(IPoint point) => Template.IsWithinMap(point);

    /// <summary>
    ///     Moves an entity within the point lookup of the master object collection. DO NOT USE THIS UNLESS YOU KNOW WHAT YOU ARE DOING.
    /// </summary>
    public void MoveEntity(MapEntity entity, Point oldPoint) => Objects.MoveEntity(entity, oldPoint);

    public void PlayMusic(byte music)
    {
        foreach (var aisling in Objects.Values<Aisling>())
            aisling.Client.SendSound(music, true);
    }

    public void PlaySound(byte sound, IPoint point)
    {
        foreach (var aisling in Objects.WithinRange<Aisling>(point))
            aisling.Client.SendSound(sound, false);
    }

    public void PlaySound(byte sound, ICollection<IPoint> points)
    {
        var aislings = Objects.Values<Aisling>()
                              .Where(aisling => points.Any(p => p.WithinRange(aisling)));

        foreach (var aisling in aislings)
            aisling.Client.SendSound(sound, false);
    }

    public bool RemoveObject(MapEntity mapEntity)
    {
        if (!Objects.Remove(mapEntity.Id))
            return false;

        if (mapEntity is VisibleEntity visibleObject)
            foreach (var aisling in Objects.WithinRange<Aisling>(visibleObject)
                                           .ThatCanSee(visibleObject))
                aisling.Client.SendRemoveObject(visibleObject.Id);

        if (mapEntity is Creature creature)
        {
            Script.OnExited(creature);

            foreach (var nearbyCreature in Objects.WithinRange<Creature>(creature))
                Helpers.HandleDeparture(creature, nearbyCreature);
        }

        return true;
    }

    public void ShowAnimation(Animation animation)
    {
        if (animation.TargetId.HasValue)
        {
            if (TryGetObject<Creature>(animation.TargetId.Value, out var target))
                foreach (var aisling in Objects.WithinRange<Aisling>(target)
                                               .ThatCanSee(target))
                    aisling.Client.SendAnimation(animation);
        } else if (animation.TargetPoint != default)
            foreach (var aisling in Objects.WithinRange<Aisling>(animation.TargetPoint))
                aisling.Client.SendAnimation(animation);
    }

    public void SimpleAdd(MapEntity mapEntity)
    {
        mapEntity.MapInstance = this;
        Objects.Add(mapEntity.Id, mapEntity);
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{{ Type: \"{nameof(MapInstance)}\", InstanceId: \"{InstanceId}\", TemplateId: {Template.MapId} }}";

    public bool TryGetObject<T>(uint id, [MaybeNullWhen(false)] out T obj) => Objects.TryGetValue(id, out obj);

    public void Update(TimeSpan delta)
    {
        Objects.Update(delta);

        foreach (ref var spawn in CollectionsMarshal.AsSpan(MonsterSpawns))
            spawn.Update(delta);

        Script.Update(delta);
        HandleShardLimitersTimer.Update(delta);

        foreach (var timer in ShardLimiterTimers.Values)
            timer.Update(delta);

        if (HandleShardLimitersTimer.IntervalElapsed)
            HandleShardLimiters();
    }
}