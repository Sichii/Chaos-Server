using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Core.JsonConverters;
using Chaos.Cryptography.Definitions;
using Chaos.DataObjects;
using Chaos.Managers.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Chaos.WorldObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public class MapTemplateManager : ICacheManager<short, MapTemplate>
{
    private readonly ConcurrentDictionary<short, MapTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly MapTemplateManagerOptions Options;

    public MapTemplateManager(IOptionsSnapshot<MapTemplateManagerOptions> options, ILogger<MapTemplateManager> logger)
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<short, MapTemplate>();

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public MapTemplate GetObject(short key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
                await LoadTemplateAsync(path, token).ConfigureAwait(false));

        Logger.LogInformation("{Count} map templates loaded", Cache.Count);
    }

    private async Task LoadMapDataAsync(MapTemplate mapTemplate)
    {
        var path = Path.Combine(Options.MapDataDirectory, $"lod{mapTemplate.TemplateKey}.map");
        var data = await File.ReadAllBytesAsync(path);
        var index = 0;

        for (var y = 0; y < mapTemplate.Height; y++)
            for (var x = 0; x < mapTemplate.Width; x++)
            {
                Point point = (x, y);
                var background = (ushort)(data[index++] | (data[index++] << 8));
                var leftForeground = (ushort)(data[index++] | (data[index++] << 8));
                var rightForeground = (ushort)(data[index++] | (data[index++] << 8));
                var tile = new Tile(background, leftForeground, rightForeground);

                mapTemplate.Tiles[x, y] = tile;

                if (CONSTANTS.DOOR_SPRITES.Contains(leftForeground))
                    mapTemplate.Doors[point] = new Door(point, leftForeground, true);
                else if (CONSTANTS.DOOR_SPRITES.Contains(rightForeground))
                    mapTemplate.Doors[point] = new Door(point, rightForeground, false);
            }

        mapTemplate.CheckSum = data.Generate16();
    }

    private async Task LoadTemplateAsync(string path, CancellationToken token)
    {
        try
        {
            var mapTemplate = await LoadTemplateFromFileAsync(path, token);
            Cache.TryAdd(mapTemplate.TemplateKey, mapTemplate);
            Logger.LogDebug("Loaded map template {MapId}", mapTemplate.TemplateKey);
        } catch (FileNotFoundException e)
        {
            var fileName = Path.GetFileName(e.FileName);
            Logger.LogWarning("Map data not found for {FileName}", fileName);
        }
    }

    private async ValueTask<MapTemplate> LoadTemplateFromFileAsync(string path, CancellationToken token)
    {
        await using var stream = File.OpenRead(path);
        var mapTemplate = await JsonSerializer.DeserializeAsync<MapTemplate>(stream, JsonSerializerOptions);
        mapTemplate!.Tiles = new Tile[mapTemplate.Width + 1, mapTemplate.Height + 1];
        await LoadMapDataAsync(mapTemplate!);

        return mapTemplate!;
    }
    
    public IEnumerator<MapTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}