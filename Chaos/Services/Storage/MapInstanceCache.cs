using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Entities.Schemas.Content;
using Chaos.Extensions;
using Chaos.Pathfinding.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class MapInstanceCache : SimpleFileCacheBase<MapInstance, MapInstanceSchema, MapInstanceCacheOptions>
{
    private readonly IPathfindingService PathfindingService;

    /// <inheritdoc />
    protected override Func<MapInstance, string> KeySelector => m => m.InstanceId;

    public MapInstanceCache(
        ITypeMapper mapper,
        IPathfindingService pathfindingService,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MapInstanceCacheOptions> options,
        ILogger<MapInstanceCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger)
    {
        PathfindingService = pathfindingService;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        AsyncHelpers.RunSync(ReloadAsync);
    }

    protected override async Task<MapInstance?> LoadFromFileAsync(string directory)
    {
        var mapInstancePath = Path.Combine(directory, "instance.json");
        var monsterSpawnsPath = Path.Combine(directory, "spawns.json");

        await using var mapInstanceStream = File.OpenRead(mapInstancePath);
        await using var monsterSpawnsStream = File.OpenRead(monsterSpawnsPath);

        var mapInstanceSchema = await JsonSerializer.DeserializeAsync<MapInstanceSchema>(mapInstanceStream, JsonSerializerOptions);
        var monsterSpawnsSchema = await JsonSerializer.DeserializeAsync<MonsterSpawnsSchema>(monsterSpawnsStream, JsonSerializerOptions);

        if (mapInstanceSchema == null)
            return null;

        var mapInstance = Mapper.Map<MapInstance>(mapInstanceSchema!);
        var spawns = Mapper.MapMany<MonsterSpawn>(monsterSpawnsSchema!);

        mapInstance.Pathfinder = PathfindingService;
        PathfindingService.RegisterGrid(mapInstance);

        foreach (var spawn in spawns)
            mapInstance.AddSpawner(spawn);

        return mapInstance;
    }
}