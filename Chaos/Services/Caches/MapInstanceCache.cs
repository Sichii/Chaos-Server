using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.World;
using Chaos.Extensions;
using Chaos.Pathfinding.Interfaces;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MapInstanceCache : ISimpleCache<MapInstance>
{
    private readonly ConcurrentDictionary<string, MapInstance> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly int Loaded;
    private readonly ILogger Logger;
    private readonly ITypeMapper Mapper;
    private readonly MapInstanceCacheOptions Options;
    private readonly IPathfindingService PathfindingService;

    public MapInstanceCache(
        ITypeMapper mapper,
        IPathfindingService pathfindingService,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MapInstanceCacheOptions> options,
        ILogger<MapInstanceCache> logger
    )
    {
        Mapper = mapper;
        PathfindingService = pathfindingService;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        if (Interlocked.CompareExchange(ref Loaded, 1, 0) == 0)
            AsyncHelpers.RunSync(LoadCacheAsync);
    }

    public IEnumerator<MapInstance> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public MapInstance GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    private async Task LoadCacheAsync()
    {
        var instances = Directory.GetDirectories(Options.Directory);

        await Parallel.ForEachAsync(
            instances,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, _) =>
            {
                var mapInstance = await LoadInstanceFromFileAsync(path);
                Cache.TryAdd(mapInstance.InstanceId, mapInstance);
                Logger.LogTrace("Loaded map instance {MapInstanceId}", mapInstance.InstanceId);
            });

        Logger.LogInformation("{Count} map instances loaded", Cache.Count);
    }

    private async Task<MapInstance> LoadInstanceFromFileAsync(string directory)
    {
        var mapInstanceFilePath = Path.Combine(directory, "instance.json");
        await using var stream = File.OpenRead(mapInstanceFilePath);
        var schema = await JsonSerializer.DeserializeAsync<MapInstanceSchema>(stream, JsonSerializerOptions);
        var mapInstance = Mapper.Map<MapInstance>(schema!);

        PathfindingService.RegisterGrid(mapInstance);

        return mapInstance;
    }
}