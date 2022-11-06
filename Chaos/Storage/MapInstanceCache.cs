using System.Text.Json;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Factories.Abstractions;
using Chaos.Objects.World;
using Chaos.Pathfinding.Abstractions;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class MapInstanceCache : SimpleFileCacheBase<MapInstance, MapInstanceSchema, MapInstanceCacheOptions>
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IPathfindingService PathfindingService;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    protected override Func<MapInstance, string> KeySelector => m => m.InstanceId;

    public MapInstanceCache(
        ITypeMapper mapper,
        IPathfindingService pathfindingService,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MapInstanceCacheOptions> options,
        ILogger<MapInstanceCache> logger,
        IMerchantFactory merchantFactory,
        ISimpleCache simpleCache
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger)
    {
        PathfindingService = pathfindingService;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        AsyncHelpers.RunSync(ReloadAsync);
    }

    protected override async Task<MapInstance?> LoadFromFileAsync(string directory)
    {
        var mapInstancePath = Path.Combine(directory, "instance.json");
        var monsterSpawnsPath = Path.Combine(directory, "monsters.json");
        var merchantSpawnsPath = Path.Combine(directory, "merchants.json");
        var warpsPath = Path.Combine(directory, "warps.json");
        var worldMapsPath = Path.Combine(directory, "worldMaps.json");

        await using var mapInstanceStream = File.OpenRead(mapInstancePath);
        await using var monsterSpawnsStream = File.OpenRead(monsterSpawnsPath);
        await using var merchantSpawnsStream = File.OpenRead(merchantSpawnsPath);
        await using var warpsStream = File.OpenRead(warpsPath);
        await using var worldMapsStream = File.OpenRead(worldMapsPath);

        var mapInstanceSchema = await JsonSerializer.DeserializeAsync<MapInstanceSchema>(mapInstanceStream, JsonSerializerOptions);

        var monsterSpawnSchemas =
            await JsonSerializer.DeserializeAsync<List<MonsterSpawnSchema>>(monsterSpawnsStream, JsonSerializerOptions);

        var merchantSpawnSchemas =
            await JsonSerializer.DeserializeAsync<List<MerchantSpawnSchema>>(merchantSpawnsStream, JsonSerializerOptions);

        var warpSchemas = await JsonSerializer.DeserializeAsync<List<WarpSchema>>(warpsStream, JsonSerializerOptions);
        var worldMapSchemas = await JsonSerializer.DeserializeAsync<List<WorldMapWarpSchema>>(worldMapsStream, JsonSerializerOptions);

        if (mapInstanceSchema == null)
            return null;

        var mapInstance = Mapper.Map<MapInstance>(mapInstanceSchema!);
        var monsterSpawns = Mapper.MapMany<MonsterSpawn>(monsterSpawnSchemas!);
        var warps = Mapper.MapMany<Warp>(warpSchemas!);

        mapInstance.Pathfinder = PathfindingService;
        PathfindingService.RegisterGrid(mapInstance);

        foreach (var monsterSpawn in monsterSpawns)
            mapInstance.AddSpawner(monsterSpawn);

        foreach (var warp in warps)
        {
            var warpTile = new WarpTile(mapInstance, SimpleCache, warp);

            mapInstance.SimpleAdd(warpTile);
        }

        foreach (var worldMapSchema in worldMapSchemas!)
        {
            var worldMap = SimpleCache.Get<WorldMap>(worldMapSchema.WorldMapKey);
            var worldMapTile = new WorldMapTile(mapInstance, worldMapSchema.Source, worldMap);

            mapInstance.SimpleAdd(worldMapTile);
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

        return mapInstance;
    }
}