using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Pathfinding.Abstractions;
using Chaos.Schemas.Content;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class ExpiringMapInstanceCache : ExpiringFileCache<MapInstance, MapInstanceSchema, MapInstanceCacheOptions>, IShardGenerator
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IPathfindingService PathfindingService;
    // ReSharper disable once NotAccessedField.Local
    private readonly Task PersistUsedMapsTask;
    private readonly PeriodicTimer PersistUsedMapsTimer;
    private readonly IReactorTileFactory ReactorTileFactory;

    /// <inheritdoc />
    public ExpiringMapInstanceCache(
        IMemoryCache cache,
        IEntityRepository entityRepository,
        IMerchantFactory merchantFactory,
        IMonsterFactory monsterFactory,
        IPathfindingService pathfindingService,
        IReactorTileFactory reactorTileFactory,
        IOptions<MapInstanceCacheOptions> options,
        ILogger<ExpiringMapInstanceCache> logger
    )
        : base(
            cache,
            entityRepository,
            options,
            logger)
    {
        MerchantFactory = merchantFactory;
        MonsterFactory = monsterFactory;
        PathfindingService = pathfindingService;
        ReactorTileFactory = reactorTileFactory;
        PersistUsedMapsTimer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        PersistUsedMapsTask = PersistUsedMaps();
    }

    #region MapInstanceCache
    private async Task PersistUsedMaps()
    {
        await Task.Yield();

        while (true)
            try
            {
                await PersistUsedMapsTimer.WaitForNextTickAsync();

                //checks each map for aislings
                //if the map has aislings on it, re-access the map to keep it in the cache
                foreach (var mapInstance in LocalLookup.Values)
                    if (mapInstance.GetEntities<Aisling>().Any())
                        Get(mapInstance.InstanceId);
            } catch (Exception e)
            {
                Logger.LogError(
                    e,
                    "Exception in {@ClassName} while attempting to persist maps that are in use",
                    nameof(ExpiringMapInstanceCache));
            }

        // ReSharper disable once FunctionNeverReturns
    }

    /// <inheritdoc />
    protected override MapInstance CreateFromEntry(ICacheEntry entry) => InnerCreateFromEntry(entry);

    private MapInstance InnerCreateFromEntry(ICacheEntry entry, string? loadFromFileKeyOverride = null)
    {
        //entryKey could be either a normal instanceId, or a shardId
        var entryKey = entry.Key.ToString();
        //if a load override is specified, entryKey is a shardId, and the loadInstanceId is the instance the shard is based on
        var loadInstanceId = loadFromFileKeyOverride ?? entryKey;
        var loadInstanceIdActual = DeconstructKeyForType(loadInstanceId!);
        //the deconstructed entryKey is the real instance id, whether it's a shard or not
        var entryKeyActual = DeconstructKeyForType(entryKey!);
        var shardId = string.IsNullOrEmpty(loadFromFileKeyOverride) ? null : entryKeyActual;

        Logger.LogTrace("Creating new {@TypeName} entry with key {@Key}", nameof(MapInstance), loadInstanceId);

        entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins));
        entry.RegisterPostEvictionCallback(RemoveValueCallback);

        var path = GetPathForKey(loadInstanceIdActual);

        //we use entry.Key.ToString()
        var mapInstance = InnerLoadFromFile(path, shardId);

        if (!mapInstance.LoadedFromInstanceId.EqualsI(loadInstanceIdActual))
        {
            var endOfPath = Path.GetFileName(path);

            throw new InvalidOperationException(
                $"File/Directory \"{endOfPath}\" does not match the instance id \"{loadInstanceIdActual}\" of the serialized {
                    nameof(MapInstance)}");
        }

        LocalLookup[entryKey!] = mapInstance;

        Logger.WithProperty(mapInstance)
              .LogInformation(
                  "Created new {@TypeName} entry with key {@Key} from path {@Path}",
                  nameof(MapInstance),
                  loadInstanceId,
                  path);

        return mapInstance;
    }

    private MapInstance InnerLoadFromFile(string directory, string? shardId = null)
    {
        var baseInstanceId = default(string?);

        var mapInstancePath = Path.Combine(directory, "instance.json");
        var monsterSpawnsPath = Path.Combine(directory, "monsters.json");
        var merchantSpawnsPath = Path.Combine(directory, "merchants.json");
        var reactorsPath = Path.Combine(directory, "reactors.json");

        Action<MapInstanceSchema>? mapInstanceAction = string.IsNullOrEmpty(shardId)
            ? null
            : schema =>
            {
                baseInstanceId = schema.InstanceId;
                schema.InstanceId = shardId;
            };

        var mapInstance = EntityRepository.LoadAndMap<MapInstance, MapInstanceSchema>(mapInstancePath, mapInstanceAction);
        var monsterSpawns = EntityRepository.LoadAndMapMany<MonsterSpawn, MonsterSpawnSchema>(monsterSpawnsPath);
        var merchantSpawnSchemas = EntityRepository.LoadMany<MerchantSpawnSchema>(merchantSpawnsPath);
        var reactorsSchemas = EntityRepository.LoadMany<ReactorTileSchema>(reactorsPath);

        mapInstance.BaseInstanceId = baseInstanceId;

        foreach (var reactorSchema in reactorsSchemas)
        {
            var owner = string.IsNullOrEmpty(reactorSchema.OwnerMonsterTemplateKey)
                ? null
                : MonsterFactory.Create(reactorSchema.OwnerMonsterTemplateKey, mapInstance, reactorSchema.Source);

            var reactor = ReactorTileFactory.Create(
                mapInstance,
                reactorSchema.Source,
                reactorSchema.ShouldBlockPathfinding,
                reactorSchema.ScriptKeys,
                new ConcurrentDictionary<string, IScriptVars>(
                    reactorSchema.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
                    StringComparer.OrdinalIgnoreCase),
                owner);

            mapInstance.SimpleAdd(reactor);
        }

        foreach (var merchantSpawn in merchantSpawnSchemas)
        {
            var merchant = MerchantFactory.Create(
                merchantSpawn.MerchantTemplateKey,
                mapInstance,
                merchantSpawn.SpawnPoint,
                merchantSpawn.ExtraScriptKeys);

            var pathingBoundsBlacklist = merchantSpawn.PathingBounds?.GetOutline() ?? Array.Empty<Point>();
            merchant.BlackList = merchantSpawn.BlackList.Concat(pathingBoundsBlacklist.OfType<IPoint>()).ToList();
            merchant.Direction = merchantSpawn.Direction ?? (Direction)Random.Shared.Next(4);
            mapInstance.SimpleAdd(merchant);
        }

        foreach (var monsterSpawn in monsterSpawns)
        {
            mapInstance.AddSpawner(monsterSpawn);
            monsterSpawn.FullSpawn();
        }

        mapInstance.Pathfinder = PathfindingService;
        PathfindingService.RegisterGrid(mapInstance);

        mapInstance.StartAsync();

        return mapInstance;
    }

    /// <inheritdoc />
    protected override void RemoveValueCallback(
        object key,
        object? value,
        EvictionReason reason,
        object? state
    )
    {
        base.RemoveValueCallback(
            key,
            value,
            reason,
            state);

        if (reason != EvictionReason.Replaced)
        {
            var mapInstance = (MapInstance)value!;
            mapInstance.Destroy();
        }
    }

    /// <inheritdoc />
    public override Task ReloadAsync()
    {
        Paths = LoadPaths();

        foreach (var key in LocalLookup.Keys)
        {
            if (!Cache.TryGetValue(key, out var value))
                continue;

            var mapInstance = (MapInstance)value!;
            var instanceId = DeconstructKeyForType(DeconstructShardKey(key));

            try
            {
                using var entry = Cache.CreateEntry(key);
                entry.Value = mapInstance.IsShard ? InnerCreateFromEntry(entry, instanceId) : InnerCreateFromEntry(entry);
            } catch (Exception e)
            {
                Logger.WithProperty(mapInstance)
                      .LogError(e, "Failed to reload map instance with key {@Key}", key);
                //otherwise ignored
            }
        }

        ReconstructShardLookups();

        return Task.CompletedTask;
    }
    #endregion

    #region ShardGenerator
    /// <inheritdoc />
    public MapInstance CreateShardOfInstance(string instanceId)
    {
        var key = ConstructKeyForType(instanceId);
        var shardKey = ConstructShardKey(key);

        using var entry = Cache.CreateEntry(shardKey);
        var ret = InnerCreateFromEntry(entry, key);
        entry.Value = ret;

        return ret;
    }

    private string ConstructShardKey(string key) => $"{key}___{Guid.NewGuid()}";

    private string DeconstructShardKey(string key) => string.Join("___", key.Split("___").SkipLast(1));

    private void ReconstructShardLookups()
    {
        var shards = LocalLookup.Values
                                .Where(m => m.IsShard)
                                .ToList();

        foreach (var mapGroup in shards.GroupBy(s => s.BaseInstanceId))
        {
            var shardLookup = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);

            foreach (var map in mapGroup)
            {
                shardLookup.TryAdd(map.InstanceId, map);
                map.Shards = shardLookup;
            }

            var key = ConstructKeyForType(mapGroup.Key!);

            LocalLookup.TryGetValue(key, out var baseMapInstance);
            baseMapInstance!.Shards = shardLookup;
        }
    }
    #endregion
}