using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Core.Extensions;
using Chaos.Objects.World;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class MapInstanceCache : ISimpleCache<string, MapInstance>
{
    private readonly ConcurrentDictionary<string, MapInstance> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ISimpleCache<string, MapTemplate> MapTemplate;
    private readonly MapInstanceManagerOptions Options;
    private readonly WorldOptions WorldOptions;

    public MapInstanceCache(
        ISimpleCache<string, MapTemplate> mapTemplate,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<WorldOptions> worldOptions,
        IOptionsSnapshot<MapInstanceManagerOptions> options,
        ILogger<MapInstanceCache> logger
    )
    {
        MapTemplate = mapTemplate;
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
        var instances = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(
            instances,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var mapInstance = await LoadInstanceFromFileAsync(path);
                Cache.TryAdd(mapInstance.InstanceId, mapInstance);
                Logger.LogTrace("Loaded map instance {MapInstanceId}", mapInstance.InstanceId);
            });

        Logger.LogInformation("{Count} map instances loaded", Cache.Count);
    }

    private async ValueTask<MapInstance> LoadInstanceFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var mapInstance = await JsonSerializer.DeserializeAsync<MapInstance>(stream, JsonSerializerOptions);
        var template = MapTemplate.GetObject(mapInstance!.MapId.ToString());
        mapInstance.Template = template;
        mapInstance.WorldOptions = WorldOptions;

        foreach (var warp in mapInstance.WarpGroups.Flatten())
            mapInstance.SimpleAdd(new WarpTile(this, warp));

        return mapInstance!;
    }
}