using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Pathfinding.Abstractions;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Content;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class ExpiringMapInstanceCache : ExpiringFileCacheBase<MapInstance, MapInstanceSchema>, IShardGenerator
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IPathfindingService PathfindingService;
    // ReSharper disable once NotAccessedField.Local
    private readonly Task PersistUsedMapsTask;
    private readonly PeriodicTimer PersistUsedMapsTimer;
    private readonly IReactorTileFactory ReactorTileFactory;

    /// <inheritdoc />
    public ExpiringMapInstanceCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IMerchantFactory merchantFactory,
        IPathfindingService pathfindingService,
        IReactorTileFactory reactorTileFactory,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringMapInstanceCacheOptions> options,
        ILogger<ExpiringMapInstanceCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger)
    {
        MerchantFactory = merchantFactory;
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
                Logger.LogCritical(
                    e,
                    "Critical exception in {ClassName} while attempting to persist maps that are in use",
                    nameof(ExpiringMapInstanceCache));
            }

        // ReSharper disable once FunctionNeverReturns
    }

    protected override MapInstance LoadFromFile(string directory) => InnerLoadFromFile(directory);

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

        Logger.LogTrace("Creating new {TypeName} entry with key \"{Key}\"", nameof(MapInstance), loadInstanceId);

        entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins));
        entry.RegisterPostEvictionCallback(RemoveValueCallback!);

        var path = GetPathForKey(loadInstanceIdActual);

        //we use entry.Key.ToString()
        var ret = InnerLoadFromFile(path, shardId);

        LocalLookup[entryKey!] = ret;

        Logger.LogTrace(
            "Created new {TypeName} entry with key \"{Key}\" from path \"{Path}\"",
            nameof(MapInstance),
            loadInstanceId,
            path);

        return ret;
    }

    private MapInstance InnerLoadFromFile(string directory, string? shardId = null)
    {
        var mapInstancePath = Path.Combine(directory, "instance.json");
        var monsterSpawnsPath = Path.Combine(directory, "monsters.json");
        var merchantSpawnsPath = Path.Combine(directory, "merchants.json");
        var reactorsPath = Path.Combine(directory, "reactors.json");

        using var mapInstanceStream = File.OpenRead(mapInstancePath);
        using var monsterSpawnsStream = File.OpenRead(monsterSpawnsPath);
        using var merchantSpawnsStream = File.OpenRead(merchantSpawnsPath);
        using var reactorsStream = File.OpenRead(reactorsPath);

        var mapInstanceSchema = JsonSerializer.Deserialize<MapInstanceSchema>(mapInstanceStream, JsonSerializerOptions);

        var monsterSpawnSchemas =
            JsonSerializer.Deserialize<List<MonsterSpawnSchema>>(monsterSpawnsStream, JsonSerializerOptions);

        var merchantSpawnSchemas =
            JsonSerializer.Deserialize<List<MerchantSpawnSchema>>(merchantSpawnsStream, JsonSerializerOptions);

        var reactorsSchemas = JsonSerializer.Deserialize<List<ReactorTileSchema>>(reactorsStream, JsonSerializerOptions);

        if (mapInstanceSchema == null)
            throw new SerializationException($"Failed to serialize {nameof(MapInstanceSchema)} from directory \"{directory}\"");

        var baseInstanceId = default(string?);

        if (!string.IsNullOrEmpty(shardId))
        {
            baseInstanceId = mapInstanceSchema.InstanceId;
            mapInstanceSchema.InstanceId = shardId;
        }

        var mapInstance = Mapper.Map<MapInstance>(mapInstanceSchema);
        mapInstance.BaseInstanceId = baseInstanceId;

        var monsterSpawns = Mapper.MapMany<MonsterSpawn>(monsterSpawnSchemas!);

        foreach (var reactorSchema in reactorsSchemas!)
        {
            var reactor = ReactorTileFactory.Create(
                mapInstance,
                reactorSchema.Source,
                reactorSchema.ShouldBlockPathfinding,
                reactorSchema.ScriptKeys,
                reactorSchema.ScriptVars);

            mapInstance.SimpleAdd(reactor);
        }

        foreach (var monsterSpawn in monsterSpawns)
        {
            mapInstance.AddSpawner(monsterSpawn);
            monsterSpawn.FullSpawn();
        }

        foreach (var merchantSpawn in merchantSpawnSchemas!)
        {
            var merchant = MerchantFactory.Create(
                merchantSpawn.MerchantTemplateKey,
                mapInstance,
                merchantSpawn.SpawnPoint,
                merchantSpawn.ExtraScriptKeys);

            merchant.Direction = merchantSpawn.Direction;
            mapInstance.SimpleAdd(merchant);
        }

        mapInstance.Pathfinder = PathfindingService;
        PathfindingService.RegisterGrid(mapInstance);

        return mapInstance;
    }

    /// <inheritdoc />
    protected override void RemoveValueCallback(
        object key,
        object value,
        EvictionReason reason,
        object state
    )
    {
        base.RemoveValueCallback(
            key,
            value,
            reason,
            state);

        if (reason != EvictionReason.Replaced)
        {
            var mapInstance = (MapInstance)value;
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

            using var entry = Cache.CreateEntry(key);
            entry.Value = mapInstance.IsShard ? InnerCreateFromEntry(entry, instanceId) : InnerCreateFromEntry(entry);
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