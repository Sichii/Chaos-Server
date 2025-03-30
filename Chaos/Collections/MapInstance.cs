#region
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Chaos.Common.Synchronization;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents an instance of a map
/// </summary>
public sealed class MapInstance : IScripted<IMapScript>, IDeltaUpdatable
{
    private readonly IAsyncStore<Aisling> AislingStore;
    private readonly IIntervalTimer DayNightCycleTimer;
    private readonly DeltaMonitor DeltaMonitor;
    private readonly DeltaTime DeltaTime;
    private readonly PeriodicTimer DeltaTimer;
    private readonly IIntervalTimer HandleShardLimitersTimer;
    private readonly ILogger<MapInstance> Logger;
    private readonly MapEntityCollection Objects;
    private readonly ConcurrentQueue<Action> ProcessingQueue;
    private readonly CancellationToken ServerShutdownToken;
    private readonly IShardGenerator ShardGenerator;
    private readonly ISimpleCache SimpleCache;

    /// <summary>
    ///     Whether the map will experience day/night cycles automatically
    /// </summary>
    public bool AutoDayNightCycle { get; set; }

    /// <summary>
    ///     If the map is a shard, this will be the instance id of the map this shard was created from
    /// </summary>
    public string? BaseInstanceId { get; set; }

    /// <summary>
    ///     The current light level being displayed on the map
    /// </summary>
    public LightLevel CurrentLightLevel { get; set; } = LightLevel.Lightest_A;

    /// <summary>
    ///     A flag, or combination of flags that should affect the map
    /// </summary>
    public MapFlags Flags { get; set; }

    /// <summary>
    ///     A unique id specific to this map instance
    /// </summary>
    public string InstanceId { get; init; }

    /// <summary>
    ///     Default null
    ///     <br />
    ///     If specified, sets the minimum level needed to enter this map via warp tile
    /// </summary>
    public int? MaximumLevel { get; set; }

    /// <summary>
    ///     Default null
    ///     <br />
    ///     If specified, sets the maximum level allowed to enter this map via warp tile
    /// </summary>
    public int? MinimumLevel { get; set; }

    /// <summary>
    ///     The byte values of the music track to play when entering the map
    ///     <br />
    ///     These values aren't explored yet, so you'll have to figure out what's available yourself
    /// </summary>
    public byte Music { get; set; }

    /// <summary>
    ///     The name of the map that will display in-game
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     A service used to calculate paths between points
    /// </summary>
    public IPathfindingService Pathfinder { get; set; } = null!;

    /// <summary>
    ///     Default null
    ///     <br />
    ///     If specified, these options will be used to determine how this instance will shard itself
    /// </summary>
    public ShardingOptions? ShardingOptions { get; init; }

    /// <summary>
    ///     A collection of timers that will remove aislings from the map if the player limit is reached
    /// </summary>
    public ConcurrentDictionary<Aisling, IIntervalTimer> ShardLimiterTimers { get; set; }

    /// <summary>
    ///     A collection of all the shards of this map
    /// </summary>
    public ConcurrentDictionary<string, MapInstance> Shards { get; set; }

    /// <summary>
    ///     The template this map is based on
    /// </summary>
    public MapTemplate Template { get; set; }

    /// <summary>
    ///     An object used to cancel execution of the map
    /// </summary>
    public CancellationTokenSource MapInstanceCtx { get; }

    /// <summary>
    ///     A collection of details about what monsters should spawn and how they should spawn
    /// </summary>
    public List<MonsterSpawn> MonsterSpawns { get; }

    /// <inheritdoc />
    public IMapScript Script { get; }

    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    /// <summary>
    ///     The synchronization mechanism used to ensure thread safety on the map
    /// </summary>
    public FifoAutoReleasingSemaphoreSlim Sync { get; }

    /// <summary>
    ///     Whether this map is a shard of another map
    /// </summary>
    public bool IsShard => !string.IsNullOrEmpty(BaseInstanceId);

    /// <summary>
    ///     The id of the map this instance was loaded from. This is populated even if the map is not a shard
    /// </summary>
    public string LoadedFromInstanceId => BaseInstanceId ?? InstanceId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MapInstance" /> class
    /// </summary>
    /// <param name="template">
    ///     The template this map is created from
    /// </param>
    /// <param name="simpleCache">
    ///     A generic cache provider
    /// </param>
    /// <param name="shardGenerator">
    ///     A services used to generate shards of maps
    /// </param>
    /// <param name="scriptProvider">
    ///     A service used to generate scripts
    /// </param>
    /// <param name="name">
    ///     The name of the map
    /// </param>
    /// <param name="instanceId">
    ///     The unique instance if of the map
    /// </param>
    /// <param name="aislingStore">
    ///     A service that stores aisling data
    /// </param>
    /// <param name="serverCtx">
    ///     An object used to signal to the map to stop executing
    /// </param>
    /// <param name="logger">
    ///     A class logger used to log messages
    /// </param>
    /// <param name="extraScriptKeys">
    ///     Any extra script keys beyond those included via the template
    /// </param>
    public MapInstance(
        MapTemplate template,
        ISimpleCache simpleCache,
        IShardGenerator shardGenerator,
        IScriptProvider scriptProvider,
        string name,
        string instanceId,
        IAsyncStore<Aisling> aislingStore,
        CancellationTokenSource serverCtx,
        ILogger<MapInstance> logger,
        ICollection<string>? extraScriptKeys = null)
    {
        Name = name;
        InstanceId = instanceId;
        AislingStore = aislingStore;
        Logger = logger;
        ServerShutdownToken = serverCtx.Token;
        SimpleCache = simpleCache;
        ProcessingQueue = new ConcurrentQueue<Action>();

        Objects = new MapEntityCollection(logger, template.Width, template.Height);

        MapInstanceCtx = CancellationTokenSource.CreateLinkedTokenSource(ServerShutdownToken);
        MonsterSpawns = [];
        Sync = new FifoAutoReleasingSemaphoreSlim(1, 1, $"MapInstance {InstanceId}");
        Template = template;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        Shards = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);
        ShardLimiterTimers = new ConcurrentDictionary<Aisling, IIntervalTimer>();
        ShardGenerator = shardGenerator;
        HandleShardLimitersTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        DayNightCycleTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        var delta = 1000.0 / WorldOptions.Instance.UpdatesPerSecond;
        DeltaTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(delta));
        DeltaTime = new DeltaTime();

        DeltaMonitor = new DeltaMonitor(
            InstanceId,
            logger,
            TimeSpan.FromMinutes(1),
            Math.Min(delta * 10, 500));

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<IMapScript, MapInstance>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        try
        {
            Objects.Update(delta);

            foreach (ref var spawn in CollectionsMarshal.AsSpan(MonsterSpawns))
                spawn.Update(delta);

            Script.Update(delta);
            HandleShardLimitersTimer.Update(delta);
            DayNightCycleTimer.Update(delta);

            foreach (var timer in ShardLimiterTimers.Values)
                timer.Update(delta);

            if (HandleShardLimitersTimer.IntervalElapsed)
                HandleShardLimiters();

            if (AutoDayNightCycle && DayNightCycleTimer.IntervalElapsed)
            {
                var lightLevel = GameTime.Now.TimeOfDay;

                if (CurrentLightLevel != lightLevel)
                {
                    CurrentLightLevel = lightLevel;

                    foreach (var aisling in Objects.Values<Aisling>())
                        aisling.Client.SendLightLevel(CurrentLightLevel);
                }
            }

            while (ProcessingQueue.TryDequeue(out var action))
                try
                {
                    action();
                } catch (Exception e)
                {
                    Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Update)
                          .LogError(e, "Failed to process action in map {@MapInstance}", this);
                }
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Update)
                  .LogError(e, "Failed to update map {@MapInstance}", this);
        }
    }

    /// <summary>
    ///     Adds an aisling to the map directly without any sharding logic
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to add to the map
    /// </param>
    /// <param name="point">
    ///     The point in which to place the aisling
    /// </param>
    /// <remarks>
    ///     This method is used when sharding should be ignored, such as admin commands, or when joining a map with group
    ///     members on it
    /// </remarks>
    public void AddAislingDirect(Aisling aisling, IPoint point) => InnerAddEntity(aisling, point);

    /// <summary>
    ///     Adds a collection of entities to the map
    /// </summary>
    /// <param name="visibleObjects">
    ///     The entities to add to the map
    /// </param>
    /// <typeparam name="T">
    ///     The type of the entities to add
    /// </typeparam>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an aisling is included in the collection
    /// </exception>
    /// <remarks>
    ///     Do not use this method to add aislings to the map. This method will handle all aspects of this operation, including
    ///     updating viewports, and invoking events on scripts
    /// </remarks>
    public void AddEntities<T>(ICollection<T> visibleObjects) where T: VisibleEntity
    {
        if (visibleObjects.Any(obj => obj is Aisling))
            throw new InvalidOperationException($"Do not call @{nameof(AddEntities)} with aislings.");

        if (visibleObjects.Count == 0)
            return;

        //set map instance of all incoming objects
        foreach (var visibleObj in visibleObjects)
        {
            visibleObj.MapInstance = this;
            Objects.Add(visibleObj.Id, visibleObj);

            if (visibleObj is Creature creature)
                Script.OnEntered(creature);
        }

        var incomingEntities = visibleObjects.ToHashSet<VisibleEntity>();

        //update the viewport of any creature within range of the new objects
        foreach (var creature in GetEntities<Creature>())
        {
            //incoming entities need full viewport updates so that they can see existing entities
            if (incomingEntities.Contains(creature))
            {
                creature.UpdateViewPort();

                continue;
            }

            //entities that were already on the map only need to be updated with the incoming entities
            //and only if there even are any incoming entities within their viewport
            var withinRange = visibleObjects.ThatAreWithinRange(creature)
                                            .OfType<VisibleEntity>()
                                            .ToHashSet();

            //non-aislings only cause partial viewport updates because they do not have shared vision requirements (due to lanterns)
            //this method should never be called with aislings
            if (withinRange.Count != 0)
                creature.UpdateViewPort(withinRange);
        }
    }

    /// <summary>
    ///     Adds an entity to the map
    /// </summary>
    /// <param name="visibleEntity">
    ///     The entity to add to the map
    /// </param>
    /// <param name="point">
    ///     The point on which to place the entity
    /// </param>
    /// <remarks>
    ///     This method will handle all aspects of this operation, including updating viewports, and invoking events on scripts
    /// </remarks>
    public void AddEntity(VisibleEntity visibleEntity, IPoint point)
    {
        //shards cant shard, shardtype none means no sharding, non-aisling cant create shards
        if (IsShard
            || (ShardingOptions == null)
            || (ShardingOptions.ShardingType == ShardingType.None)
            || visibleEntity is not Aisling aisling)
            InnerAddEntity(visibleEntity, point);
        else
            HandleSharding(aisling, point);
    }

    /// <summary>
    ///     Adds a monster spawner to the map
    /// </summary>
    /// <param name="monsterSpawn">
    ///     The spawner to add
    /// </param>
    public void AddSpawner(MonsterSpawn monsterSpawn)
    {
        monsterSpawn.MapInstance = this;
        MonsterSpawns.Add(monsterSpawn);
    }

    private void AddToNewShard(Aisling aisling, IPoint point)
    {
        var shard = ShardGenerator.CreateShardOfInstance(InstanceId);
        shard.Shards = Shards;
        Shards.TryAdd(shard.InstanceId, shard);

        shard.AddAislingDirect(aisling, point);
    }

    /// <summary>
    ///     Use this to queue up actions that need to be performed within the map's synchronization.
    /// </summary>
    /// <param name="action">
    ///     The action to perform within the map's synchronization
    /// </param>
    public void BeginInvoke(Action action) => ProcessingQueue.Enqueue(action);

    /// <summary>
    ///     Invokes a click event on an entity
    /// </summary>
    /// <param name="id">
    ///     The id of the entity to click
    /// </param>
    /// <param name="source">
    ///     The aisling that performed the click
    /// </param>
    public void Click(uint id, Aisling source)
    {
        if (TryGetEntity<VisibleEntity>(id, out var obj))
            if (obj.WithinRange(source) && source.CanObserve(obj))
                obj.OnClicked(source);
    }

    /// <summary>
    ///     Invokes a click event on a point on the map
    /// </summary>
    /// <param name="point">
    ///     The point to click
    /// </param>
    /// <param name="source">
    ///     The aisling that performed the click
    /// </param>
    public void Click(IPoint point, Aisling source)
    {
        if (!source.WithinRange(point))
            return;

        var door = GetEntitiesAtPoints<Door>(point)
                   .ThatAreObservedBy(source)
                   .TopOrDefault();

        if (door != null)
            door.OnClicked(source);
        else
        {
            var obj = GetEntitiesAtPoints<ReactorTile>(point)
                .TopOrDefault();

            obj?.OnClicked(source);
        }
    }

    /// <summary>
    ///     Destroys the map instance
    /// </summary>
    /// <remarks>
    ///     Clears all objects and spawners from the map, stops execution of the map, and removes this map from the collection
    ///     of shards. Any aislings on the map will be in a sort of limbo and need to relog
    /// </remarks>
    public void Destroy()
    {
        Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Delete)
              .WithProperty(this)
              .LogInformation("Shutting down map instance {@MapInstanceId}", InstanceId);

        Stop();
        Shards.Remove(InstanceId, out _);
        Objects.Clear();
        MonsterSpawns.Clear();
    }

    /// <summary>
    ///     Gets all distinct reactors on a given point
    /// </summary>
    /// <param name="point">
    ///     The point from which to retrieve reactors
    /// </param>
    /// <remarks>
    ///     This method will return all non-templated reactors, and only the first instance of each templated reactor
    /// </remarks>
    public IEnumerable<ReactorTile> GetDistinctReactorsAtPoint(IPoint point)
    {
        //get reactors in order of oldest to newest
        var reactors = GetEntitiesAtPoints<ReactorTile>(point)
            .OrderBy(entity => entity.Creation);
        var distinctTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //returns all static reactor tiles, and only unique templated reactor tiles
        return reactors.Where(
            reactor => reactor is not TemplatedReactorTile templatedReactorTile
                       || distinctTemplateKeys.Add(templatedReactorTile.Template.TemplateKey));
    }

    /// <summary>
    ///     Gets all entities of a specific type
    /// </summary>
    /// <typeparam name="T">
    ///     The type of entities to retrieve
    /// </typeparam>
    public IEnumerable<T> GetEntities<T>() where T: MapEntity => Objects.Values<T>();

    /// <summary>
    ///     Gets all entities at the specified points
    /// </summary>
    /// <param name="points">
    ///     The points from which to retrieve entities from
    /// </param>
    /// <typeparam name="T">
    ///     The type of entities to retrieve
    /// </typeparam>
    public IEnumerable<T> GetEntitiesAtPoints<T>(params IEnumerable<IPoint> points) where T: MapEntity
        => GetEntitiesAtPoints<T>(points.Select(Point.From));

    /// <summary>
    ///     Gets all entities at the specified points
    /// </summary>
    /// <param name="points">
    ///     The points from which to retrieve entities from
    /// </param>
    /// <typeparam name="T">
    ///     The type of entities to retrieve
    /// </typeparam>
    [OverloadResolutionPriority(1)]
    public IEnumerable<T> GetEntitiesAtPoints<T>(params IEnumerable<Point> points) where T: MapEntity => Objects.AtPoints<T>(points);

    /// <summary>
    ///     Gets all entities within the given range of a given point
    /// </summary>
    /// <param name="point">
    ///     The point from which to search
    /// </param>
    /// <param name="range">
    ///     The range to search within
    /// </param>
    /// <typeparam name="T">
    ///     The type of entities to retrieve
    /// </typeparam>
    public IEnumerable<T> GetEntitiesWithinRange<T>(IPoint point, int range = 15) where T: MapEntity
        => Objects.WithinRange<T>(point, range);

    private void HandleSharding(Aisling aisling, IPoint point)
    {
        switch (ShardingOptions!.ShardingType)
        {
            case ShardingType.None:
                InnerAddEntity(aisling, point);

                break;
            case ShardingType.PlayerLimit:
            case ShardingType.AbsolutePlayerLimit:
            {
                //if the limit is 1, do not re-use instances
                if (ShardingOptions.Limit == 1) { } else
                {
                    //non-absolute player limit will allow group members to go over the normal player limit
                    if (ShardingOptions.ShardingType == ShardingType.PlayerLimit)
                    {
                        var shard = aisling.Group
                                           ?.Where(a => !a.Equals(aisling))
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
                        var playerCount = instance.Objects
                                                  .Values<Aisling>()
                                                  .Count();

                        if (playerCount < ShardingOptions.Limit)
                        {
                            instance.InnerAddEntity(aisling, point);

                            return;
                        }
                    }

                    //if there is no available spots, create a new shard
                }

                AddToNewShard(aisling, point);

                break;
            }
            case ShardingType.AbsoluteGroupLimit:
            {
                //if any group member is already in this instance or a shard of this instance
                var shard = aisling.Group
                                   ?.Where(a => !a.Equals(aisling))
                                   .Select(m => m.MapInstance)
                                   .FirstOrDefault(
                                       m => m.InstanceId.EqualsI(InstanceId) || (m.IsShard && m.BaseInstanceId!.EqualsI(InstanceId)));

                if (shard != null)
                {
                    //add this player to that instance or shard
                    shard.AddAislingDirect(aisling, point);

                    return;
                }

                //if the limit is 1, do not re-use instances
                if (ShardingOptions.Limit == 1) { } else
                    foreach (var instance in Shards.Values.Prepend(this))
                    {
                        //get the number of groups in this instance
                        var groupCount = instance.Objects
                                                 .Values<Aisling>()
                                                 .GroupBy(a => a.Group)
                                                 .Sum(grp => grp.Key == null ? grp.Count() : 1);

                        //if this instance isnt at the group limit
                        if (groupCount < ShardingOptions.Limit)
                        {
                            //add this player to that instance and return
                            instance.InnerAddEntity(aisling, point);

                            return;
                        }
                    }

                //if we couldnt find a suitable instance to place the player, generate a new one and add them to it
                AddToNewShard(aisling, point);

                break;
            }
            case ShardingType.AbsoluteGuildLimit:
            {
                //if any guild member is already in this instance or a shard of this instance
                var shard = aisling.Guild
                                   ?.GetOnlineMembers()
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

                //if the limit is 1, do not re-use instances
                if (ShardingOptions.Limit == 1) { } else
                    foreach (var instance in Shards.Values.Prepend(this))
                    {
                        //get the number of guilds in this instance
                        var guildCount = instance.Objects
                                                 .Values<Aisling>()
                                                 .GroupBy(a => a.Guild)
                                                 .Sum(gld => gld.Key == null ? gld.Count() : 1);

                        //if this instance isnt at the guild limit
                        if (guildCount < ShardingOptions.Limit)
                        {
                            //add this player to that instance and return
                            instance.InnerAddEntity(aisling, point);

                            return;
                        }
                    }

                //if we couldnt find a suitable instance to place the player, generate a new one and add them to it
                AddToNewShard(aisling, point);

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
                var aislings = Objects.Values<Aisling>()
                                      .Where(aisling => !aisling.IsAdmin)
                                      .ToList();

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
                    ShardLimiterTimers.TryAdd(
                        newAisling,
                        new PeriodicMessageTimer(
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(1),
                            "You will be removed from the map in {Time}",
                            message => newAisling.SendActiveMessage(message)));

                    newAisling.SendActiveMessage("The map has reached it's player limit");
                    newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
                }

                //for each timer that has expired
                //move the player to the exit and remove the timer
                foreach (var kvp in ShardLimiterTimers.IntersectBy(aislingsToRemove, kvp => kvp.Key)
                                                      .ToList())
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
                var aislings = Objects.Values<Aisling>()
                                      .Where(aisling => !aisling.IsAdmin)
                                      .ToList();

                //number of unique groups in the zone
                var groups = aislings.GroupBy(a => a.Group)
                                     .SelectMany(
                                         grp =>
                                         {
                                             if (grp.Key == null)
                                                 return grp.Select(
                                                     m => new List<Aisling>
                                                     {
                                                         m
                                                     });

                                             return
                                             [
                                                 grp.Where(m => m.MapInstance == this)
                                                    .ToList()
                                             ];
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

                var aislingsToRemove = groupsToRemove.SelectMany(l => l)
                                                     .ToList();

                //for each timer that isnt for one of these aislings
                //remove that timer
                foreach (var aislingToRemove in ShardLimiterTimers.Keys.Except(aislingsToRemove))
                    ShardLimiterTimers.Remove(aislingToRemove, out _);

                //for each aisling that doesnt have a timer
                //create a timer and send an initial warning
                foreach (var newAisling in aislingsToRemove.Except(ShardLimiterTimers.Keys))
                {
                    ShardLimiterTimers.TryAdd(
                        newAisling,
                        new PeriodicMessageTimer(
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(1),
                            "You will be removed from the map in {Time}",
                            message => newAisling.SendActiveMessage(message)));

                    newAisling.SendActiveMessage("The map has reached it's group limit");
                    newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
                }

                //for each timer that has expired
                //move the player to the exit and remove the timer
                foreach (var kvp in ShardLimiterTimers.IntersectBy(aislingsToRemove, kvp => kvp.Key)
                                                      .ToList())
                    if (kvp.Value.IntervalElapsed)
                    {
                        var exitMapInstance = SimpleCache.Get<MapInstance>(ShardingOptions.ExitLocation.Map);
                        kvp.Key.TraverseMap(exitMapInstance, ShardingOptions.ExitLocation);
                        ShardLimiterTimers.Remove(kvp.Key, out _);
                    }

                break;
            }
            case ShardingType.AbsoluteGuildLimit:
            {
                var aislings = Objects.Values<Aisling>()
                                      .Where(aisling => !aisling.IsAdmin)
                                      .ToList();

                //number of unique groups in the zone
                var guilds = aislings.GroupBy(a => a.Guild)
                                     .SelectMany(
                                         gld =>
                                         {
                                             if (gld.Key == null)
                                                 return gld.Select(
                                                     m => new List<Aisling>
                                                     {
                                                         m
                                                     });

                                             return
                                             [
                                                 gld.Where(m => m.MapInstance == this)
                                                    .ToList()
                                             ];
                                         })
                                     .ToList();

                var guildCount = guilds.Count;

                var amountOverLimit = guildCount - limit;

                //if we're not over the limit, do nothing
                if (amountOverLimit <= 0)
                    return;

                var guildsToRemove = guilds.OrderByDescending(gld => gld.Max(a => a.Id))
                                           .ThenBy(gld => gld.Count)
                                           .Take(amountOverLimit)
                                           .ToList();

                var aislingsToRemove = guildsToRemove.SelectMany(l => l)
                                                     .ToList();

                //for each timer that isnt for one of these aislings
                //remove that timer
                foreach (var aislingToRemove in ShardLimiterTimers.Keys.Except(aislingsToRemove))
                    ShardLimiterTimers.Remove(aislingToRemove, out _);

                //for each aisling that doesnt have a timer
                //create a timer and send an initial warning
                foreach (var newAisling in aislingsToRemove.Except(ShardLimiterTimers.Keys))
                {
                    ShardLimiterTimers.TryAdd(
                        newAisling,
                        new PeriodicMessageTimer(
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(1),
                            "You will be removed from the map in {Time}",
                            message => newAisling.SendActiveMessage(message)));

                    newAisling.SendActiveMessage("The map has reached it's guild limit");
                    newAisling.SendActiveMessage("You will be removed from the map in 15 seconds");
                }

                //for each timer that has expired
                //move the player to the exit and remove the timer
                foreach (var kvp in ShardLimiterTimers.IntersectBy(aislingsToRemove, kvp => kvp.Key)
                                                      .ToList())
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

    private void InnerAddEntity(VisibleEntity visibleEntity, IPoint point)
    {
        visibleEntity.SetLocation(this, point);
        Objects.Add(visibleEntity.Id, visibleEntity);

        if (visibleEntity is Creature c)
        {
            if (visibleEntity is Aisling aisling)
            {
                aisling.Client.SendMapChangePending();
                aisling.Client.SendMapInfo();
                aisling.Client.SendLocation();

                //if something is shown to this aisling, the client must know they are on this map before it will show it to them
                //so we must call OnEntered after sending mapchange/mapinfo/location
                Script.OnEntered(c);

                //incoming entity needs full viewport updates so that they can see existing entities
                aisling.UpdateViewPort();

                aisling.Client.SendMapChangeComplete();

                //only send sound if the music is different
                if (!string.IsNullOrEmpty(aisling.Trackers.LastMapInstanceId))
                {
                    var lastMap = SimpleCache.Get<MapInstance>(aisling.Trackers.LastMapInstanceId);

                    if (Music != lastMap.Music)
                        aisling.Client.SendSound(Music, true);
                } else
                    aisling.Client.SendSound(Music, true);

                aisling.Client.SendMapLoadComplete();
                aisling.Client.SendDisplayAisling(aisling);
                aisling.Client.SendLightLevel(CurrentLightLevel);
            } else //incoming entity needs full viewport updates so that they can see existing entities
            {
                Script.OnEntered(c);
                c.UpdateViewPort();
            }
        }

        //if an aisling is being added to the map
        //and the creature is an aisling
        //do a full viewport update
        //otherwise just do a partial update
        foreach (var creature in GetEntitiesWithinRange<Creature>(point))
            if (creature.Equals(visibleEntity))

                // ReSharper disable once RedundantJumpStatement
                continue;
            else if (visibleEntity is Aisling && creature is Aisling)
                creature.UpdateViewPort();
            else
                creature.UpdateViewPort([visibleEntity]);
    }

    /// <summary>
    ///     Asynchronously invokes an action to be performed within the map's synchronization. Only use this if you
    ///     reeeaaalllly know what you are doing
    /// </summary>
    /// <param name="action">
    ///     The action to perform within the map's synchronization
    /// </param>
    /// <returns>
    ///     A task that will return when the action has completed.
    /// </returns>
    public Task InvokeAsync(Action action)
    {
        var tcs = new TaskCompletionSource();

        ProcessingQueue.Enqueue(DoActionWithNotify);

        return tcs.Task;

        void DoActionWithNotify()
        {
            try
            {
                action();
            } catch (Exception e)
            {
                tcs.TrySetException(e);
            } finally
            {
                tcs.TrySetResult();
            }
        }
    }

    /// <summary>
    ///     Determines if a reactor that blocks pathfinding is on a point
    /// </summary>
    /// <param name="point">
    ///     The point on which to search
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a blocking reactor is on the point, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsBlockingReactor(IPoint point)
        => GetEntitiesAtPoints<ReactorTile>(point)
            .Any(reactor => reactor.ShouldBlockPathfinding);

    /// <summary>
    ///     Determines if an entity is within the shared vision of a lantern
    /// </summary>
    /// <param name="entity">
    ///     The entity to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the entity is within the vision of any lantern, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsInSharedLanternVision(VisibleEntity entity)
    {
        if (!Flags.HasFlag(MapFlags.Darkness))
            return true;

        return GetEntitiesWithinRange<Aisling>(entity, 5)
            .Any(aisling => aisling.Illuminates(entity));
    }

    /// <summary>
    ///     Determines if a point has a reactor on it
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a reactor is on the point, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsReactor(IPoint point)
        => GetEntitiesAtPoints<ReactorTile>(point)
            .Any();

    /*/// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="creatureType">
    ///     The type of the creature
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether to ignore blocking reactors. Default behavior ignores blocking reactors only for Aislings
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(IPoint point, CreatureType creatureType, bool? ignoreBlockingReactors = null)
        => IsWalkable(Point.From(point), creatureType, ignoreBlockingReactors);*/

    /*/// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="creatureType">
    ///     The type of the creature
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether to ignore blocking reactors. Default behavior ignores blocking reactors only for Aislings
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(Point point, CreatureType creatureType, bool? ignoreBlockingReactors = null)
    {
        ignoreBlockingReactors ??= creatureType == CreatureType.Aisling;

        var creatures = GetEntitiesAtPoints<Creature>(point)
            .ToList();

        if (!ignoreBlockingReactors.Value && IsBlockingReactor(point))
            return false;

        return creatureType switch
        {
            CreatureType.Normal      => !IsWall(point) && !creatures.Any(c => creatureType.WillCollideWith(c)),
            CreatureType.WalkThrough => IsWithinMap(point) && !creatures.Any(c => creatureType.WillCollideWith(c)),
            CreatureType.Merchant    => !IsWall(point) && !creatures.Any(c => creatureType.WillCollideWith(c)),
            CreatureType.WhiteSquare => !IsWall(point) && !creatures.Any(c => creatureType.WillCollideWith(c)),
            CreatureType.Aisling     => !IsWall(point) && !creatures.Any(c => creatureType.WillCollideWith(c)),
            _                        => throw new ArgumentOutOfRangeException(nameof(creatureType), creatureType, null)
        };
    }*/

    /// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="creatureType">
    ///     The type of the creature
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(Point point, CreatureType creatureType) => IsWalkable(point, collisionType: creatureType);

    /// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="creatureType">
    ///     The type of the creature
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(IPoint point, CreatureType creatureType) => IsWalkable(Point.From(point), collisionType: creatureType);

    /// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether to ignore blocking reactors. Default behavior ignores blocking reactors only for Aislings
    /// </param>
    /// <param name="collisionType">
    ///     The type of the creature
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether to ignore walls. Default behavior ignores walls only for WalkThrough creatures
    /// </param>
    /// <param name="ignoreCollision">
    ///     Whether to ignore creature type collision. Default behavior does not ignore collision
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(
        IPoint point,
        bool? ignoreBlockingReactors = null,
        bool? ignoreWalls = null,
        bool? ignoreCollision = null,
        CreatureType? collisionType = null)
        => IsWalkable(
            Point.From(point),
            ignoreBlockingReactors,
            ignoreWalls,
            ignoreCollision,
            collisionType);

    /// <summary>
    ///     Determines if a point is walkable
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether to ignore blocking reactors. Default behavior ignores blocking reactors only for Aislings
    /// </param>
    /// <param name="collisionType">
    ///     The type of the creature
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether to ignore walls. Default behavior ignores walls only for WalkThrough creatures
    /// </param>
    /// <param name="ignoreCollision">
    ///     Whether to ignore creature type collision. Default behavior does not ignore collision
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, and walkable to the specified creature type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the creature type is not recognized
    /// </exception>
    /// <remarks>
    ///     This method checks if a point is within the map, is a wall, or has a reactor or creature that will stop you from
    ///     walking
    /// </remarks>
    public bool IsWalkable(
        Point point,
        bool? ignoreBlockingReactors = null,
        bool? ignoreWalls = null,
        bool? ignoreCollision = null,
        CreatureType? collisionType = null)
    {
        collisionType ??= CreatureType.Normal;
        ignoreBlockingReactors ??= collisionType == CreatureType.Aisling;
        ignoreWalls ??= collisionType == CreatureType.WalkThrough;
        ignoreCollision ??= false;

        var creatures = GetEntitiesAtPoints<Creature>(point)
            .ToList();

        if (!ignoreBlockingReactors.Value && IsBlockingReactor(point))
            return false;

        if (!IsWithinMap(point))
            return false;

        if (!ignoreWalls.Value && IsWall(point))
            return false;

        if (ignoreCollision.Value)
            return true;

        return !creatures.Any(c => collisionType.Value.WillCollideWith(c));
    }

    /// <summary>
    ///     Determines if a point is a wall
    /// </summary>
    /// <param name="point">
    ///     The point in which to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is a wall or is not within the map, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsWall(IPoint point) => IsWall(Point.From(point));

    /// <summary>
    ///     Determines if a point is a wall
    /// </summary>
    /// <param name="point">
    ///     The point in which to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is a wall or is not within the map, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsWall(Point point)
    {
        var door = GetEntitiesAtPoints<Door>(point)
            .FirstOrDefault();

        //if the spot is a wall
        //prevent them from walking
        if (Template.IsWall(point))
            return true;

        //if there's an open door (even if that door is considered a wall)
        //allow them to walk on the spot
        if (door is { Closed: false })
            return false;

        //if there's a closed door return false
        //otherwise return true
        return door?.Closed ?? false;
    }

    /// <summary>
    ///     Determines if a point is within the map
    /// </summary>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the point is within the map, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool IsWithinMap(IPoint point) => Template.IsWithinMap(point);

    /// <summary>
    ///     Changes the map instance's template to a new template
    /// </summary>
    /// <param name="newMapTemplateKey">
    ///     The key of the new map template to use
    /// </param>
    /// <remarks>
    ///     This method will update the map template, and refresh all aislings on the map.
    /// </remarks>
    public void Morph(string newMapTemplateKey)
    {
        var newMapTemplate = SimpleCache.Get<MapTemplate>(newMapTemplateKey);

        Script.OnMorphing(newMapTemplate);

        Template = newMapTemplate;

        foreach (var aisling in Objects.Values<Aisling>())
            aisling.Refresh(true);

        Script.OnMorphed();
    }

    /// <summary>
    ///     Moves an entity to a new point
    /// </summary>
    /// <param name="entity">
    ///     The entity to move
    /// </param>
    /// <param name="newPoint">
    ///     The point to move the entity to
    /// </param>
    /// <remarks>
    ///     This is a low-level part of the api. This method is not intended for general use
    /// </remarks>
    public void MoveEntity(MapEntity entity, Point newPoint) => Objects.MoveEntity(entity, newPoint);

    /// <summary>
    ///     Plays the specified music for all aislings on the map
    /// </summary>
    /// <param name="music">
    ///     The byte value of the music to play
    /// </param>
    public void PlayMusic(byte music)
    {
        foreach (var aisling in Objects.Values<Aisling>())
            aisling.Client.SendSound(music, true);
    }

    /// <summary>
    ///     Plays the specified sound for all near the specified points
    /// </summary>
    /// <param name="sound">
    ///     The sound to play
    /// </param>
    /// <param name="points">
    ///     The points around which to play the sound
    /// </param>
    /// <remarks>
    ///     This method ensure that the sound is only played once per aisling near any of the points
    /// </remarks>
    public void PlaySound(byte sound, params IReadOnlyList<IPoint> points)
        => PlaySound(
            sound,
            points.Select(Point.From)
                  .ToArray());

    /// <summary>
    ///     Plays the specified sound for all near the specified points
    /// </summary>
    /// <param name="sound">
    ///     The sound to play
    /// </param>
    /// <param name="points">
    ///     The points around which to play the sound
    /// </param>
    /// <remarks>
    ///     This method ensure that the sound is only played once per aisling near any of the points
    /// </remarks>
    [OverloadResolutionPriority(1)]
    public void PlaySound(byte sound, params IReadOnlyList<Point> points)
    {
        switch (points)
        {
            case []:
                return;
            case [var pt]:
                foreach (var aisling in Objects.WithinRange<Aisling>(pt))
                    aisling.Client.SendSound(sound, false);

                break;
            default:
                var aislings = Objects.Values<Aisling>()
                                      .Where(aisling => points.Any(p => p.WithinRange(aisling)));

                foreach (var aisling in aislings)
                    aisling.Client.SendSound(sound, false);

                break;
        }
    }

    /// <summary>
    ///     Removes an entity from the map
    /// </summary>
    /// <param name="mapEntity">
    ///     The entity to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the entity was found an removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool RemoveEntity(MapEntity mapEntity)
    {
        if (!Objects.Remove(mapEntity.Id))
            return false;

        switch (mapEntity)
        {
            case Aisling:
                UpdateNearbyViewPorts(mapEntity);

                break;

            //non-aislings only cause partial viewport updates because they do not have shared vision requirements (due to lanterns)
            case VisibleEntity ve:
                UpdateNearbyViewPorts(mapEntity, [ve]);

                break;
        }

        if (mapEntity is Creature c)
        {
            c.HandleMapDeparture();
            Script.OnExited(c);
        }

        return true;
    }

    /// <summary>
    ///     Shows an animation to all aislings within range of it
    /// </summary>
    /// <param name="animation">
    ///     The animation to show
    /// </param>
    /// <remarks>
    ///     If the animation provided has both a target point and target id, it will prefer to animated the point. If the
    ///     animation has neither, it will do nothing
    /// </remarks>
    public void ShowAnimation(Animation animation)
    {
        //if both target point and target id are set, prefer the point animation.
        if (animation is { TargetPoint: not null, TargetId: not null })
            animation = animation.GetPointAnimation(animation.TargetPoint);

        if (animation.TargetPoint.HasValue)
            foreach (var aisling in Objects.WithinRange<Aisling>(animation.TargetPoint))
                aisling.Client.SendAnimation(animation);
        else if (animation.TargetId.HasValue)
            if (TryGetEntity<Creature>(animation.TargetId.Value, out var target))
                foreach (var aisling in Objects.WithinRange<Aisling>(target)
                                               .ThatCanObserve(target))
                    aisling.Client.SendAnimation(animation);
    }

    /// <summary>
    ///     Adds an entity to the collection
    /// </summary>
    /// <param name="mapEntity">
    ///     The entity to add
    /// </param>
    /// <remarks>
    ///     This method will add the entity to the underlying entity collection with no additional logic. This method is not
    ///     intended for general use
    /// </remarks>
    public void SimpleAdd(MapEntity mapEntity)
    {
        mapEntity.MapInstance = this;
        Objects.Add(mapEntity.Id, mapEntity);
    }

    /// <summary>
    ///     Begins execution of the map's update loop
    /// </summary>
    public void StartAsync()
        => Task.Run(
            async () =>
            {
                var linkedCancellationToken = MapInstanceCtx.Token;

                while (true)
                {
                    if (linkedCancellationToken.IsCancellationRequested)
                        return;

                    try
                    {
                        await DeltaTimer.WaitForNextTickAsync(linkedCancellationToken);
                    } catch (OperationCanceledException)
                    {
                        return;
                    }

                    try
                    {
                        await UpdateMapAsync(DeltaTime.GetDelta);
                    } catch (Exception e)
                    {
                        Logger.WithTopics(Topics.Entities.MapInstance, Topics.Actions.Update)
                              .LogError(e, "Update succeeded, but some other error occurred for map {@MapInstance}", this);
                    }
                }
            });

    /// <summary>
    ///     Stops execution of the map's update loop
    /// </summary>
    public void Stop() => MapInstanceCtx.Cancel();

    /// <summary>
    ///     Attempts to retrieve an entity from the map by it's id
    /// </summary>
    /// <param name="id">
    ///     The id of the entity to find
    /// </param>
    /// <param name="obj">
    ///     The entity if found
    /// </param>
    /// <typeparam name="T">
    ///     The type of entity to find or cast to
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the entity was found and was able to be case to the specified type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryGetEntity<T>(uint id, [MaybeNullWhen(false)] out T obj) => Objects.TryGetValue(id, out obj);

    /// <summary>
    ///     Attempts to find a random walkable point on the map
    /// </summary>
    /// <param name="point">
    ///     A random point if found
    /// </param>
    /// <param name="creatureType">
    ///     The type of the creature. This is used to determine if a point is walkable
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a walkable point was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryGetRandomWalkablePoint([NotNullWhen(true)] out Point? point, CreatureType creatureType = CreatureType.Normal)
    {
        if (!Template.Bounds.TryGetRandomPoint(pt => IsWalkable(pt, collisionType: creatureType), out point))
            return false;

        return true;
    }

    /// <summary>
    ///     Asynchronously updates the map
    /// </summary>
    /// <param name="delta">
    ///     The delta value to update the map with
    /// </param>
    public async Task UpdateMapAsync(TimeSpan delta)
    {
        await using var sync = await Sync.WaitAsync();

        DeltaMonitor.Update(delta);

        var start = Stopwatch.GetTimestamp();

        Update(delta);

        var aislingsToSave = Objects.Values<Aisling>()
                                    .Where(aisling => aisling.SaveTimer.IntervalElapsed);

        await Task.WhenAll(aislingsToSave.Select(AislingStore.SaveAsync));

        var elapsed = Stopwatch.GetElapsedTime(start);
        DeltaMonitor.DigestDelta(elapsed);
    }

    /// <summary>
    ///     Updates the viewports of all creatures within range of a point
    /// </summary>
    /// <param name="point">
    ///     The point around which viewports are updated
    /// </param>
    /// <param name="partialUpdateEntities">
    ///     If the entities that changed are known, they are passed in to reduce computation cost of the update
    /// </param>
    public void UpdateNearbyViewPorts(IPoint point, HashSet<VisibleEntity>? partialUpdateEntities = null)
    {
        foreach (var creature in GetEntitiesWithinRange<Creature>(point))
            creature.UpdateViewPort(partialUpdateEntities);
    }
}