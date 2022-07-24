using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class MapInstanceCache : ISimpleCache<MapInstance>
{
    private readonly ConcurrentDictionary<string, MapInstance> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ISimpleCache<MapTemplate> MapTemplateCache;
    private readonly MapInstanceCacheOptions Options;
    private readonly WorldOptions WorldOptions;

    public MapInstanceCache(
        ISimpleCache<MapTemplate> mapTemplateCache,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<WorldOptions> worldOptions,
        IOptionsSnapshot<MapInstanceCacheOptions> options,
        ILogger<MapInstanceCache> logger
    )
    {
        MapTemplateCache = mapTemplateCache;
        WorldOptions = worldOptions.Value;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
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

    public async Task LoadCacheAsync()
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
        var mapInstance = await JsonSerializer.DeserializeAsync<MapInstance>(stream, JsonSerializerOptions);
        var template = MapTemplateCache.GetObject(mapInstance!.MapId.ToString());
        mapInstance.Template = template;
        mapInstance.WorldOptions = WorldOptions;

        foreach (var door in template.Doors.Values)
        {
            var cloned = new Door
            {
                Sprite = door.Sprite, 
                Closed = door.Closed
            };
            cloned.SetLocation(mapInstance, door);
            mapInstance.SimpleAdd(cloned);
        }

        foreach (var warp in mapInstance.WarpGroups.Flatten())
        {
            var cloned = new WarpTile(this) { Warp = warp };
            cloned.SetLocation(mapInstance, warp.SourceLocation!.Value);

            mapInstance.SimpleAdd(cloned);
        }
        
        return mapInstance!;
    }
}