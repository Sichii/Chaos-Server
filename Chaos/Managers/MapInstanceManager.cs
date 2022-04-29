using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Core.Extensions;
using Chaos.Core.JsonConverters;
using Chaos.Managers.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Chaos.WorldObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public class MapInstanceManager : ICacheManager<string, MapInstance>
{
    private readonly ConcurrentDictionary<string, MapInstance> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly WorldOptions WorldOptions;
    private readonly ILogger Logger;
    private readonly ICacheManager<short, MapTemplate> MapTemplateManager;
    private readonly MapInstanceManagerOptions Options;

    public MapInstanceManager(
        ICacheManager<short, MapTemplate> mapTemplateManager,
        IOptionsSnapshot<WorldOptions> worldOptions,
        IOptionsSnapshot<MapInstanceManagerOptions> options,
        ILogger<MapInstanceManager> logger)
    {
        MapTemplateManager = mapTemplateManager;
        WorldOptions = worldOptions.Value;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, MapInstance>(StringComparer.OrdinalIgnoreCase);

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public MapInstance GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var instances = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(instances,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var mapInstance = await LoadFromFileAsync(path, token);
                Cache.TryAdd(mapInstance.InstanceId, mapInstance);
                Logger.LogDebug("Loaded map instance {MapInstanceId}", mapInstance.InstanceId);
            });

        Logger.LogInformation("{Count} map instances loaded", Cache.Count);
    }

    private async ValueTask<MapInstance> LoadFromFileAsync(string path, CancellationToken token)
    {
        await using var stream = File.OpenRead(path);
        var mapInstance = await JsonSerializer.DeserializeAsync<MapInstance>(stream, JsonSerializerOptions);
        var template = MapTemplateManager.GetObject(mapInstance!.MapId);
        mapInstance.Template = template;
        mapInstance.WorldOptions = WorldOptions;

        foreach (var warp in mapInstance.WarpGroups.Flatten())
            mapInstance.SimpleAdd(new WarpTile(this, warp));
        
        return mapInstance!;
    }
    
    public IEnumerator<MapInstance> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}