using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Core.Utilities;
using Chaos.Cryptography.Extensions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MapTemplateCache : ISimpleCache<MapTemplate>
{
    private readonly ConcurrentDictionary<string, MapTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly int Loaded;
    private readonly ILogger Logger;
    private readonly ITypeMapper Mapper;
    private readonly string NeedsMapDataDir;
    private readonly MapTemplateCacheOptions Options;

    public MapTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MapTemplateCacheOptions> options,
        ILogger<MapTemplateCache> logger
    )
    {
        Mapper = mapper;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, MapTemplate>();
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        NeedsMapDataDir = Path.Combine(Options.Directory, "NeedsMapData");

        if (!Directory.Exists(NeedsMapDataDir))
            Directory.CreateDirectory(NeedsMapDataDir);

        if (Interlocked.CompareExchange(ref Loaded, 1, 0) == 0)
            AsyncHelpers.RunSync(LoadCacheAsync);
    }

    public IEnumerator<MapTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public MapTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    private async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(
            templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, _) =>
                await LoadTemplateAsync(path).ConfigureAwait(false));

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
                var point = new Point(x, y);
                var background = (ushort)(data[index++] | (data[index++] << 8));
                var leftForeground = (ushort)(data[index++] | (data[index++] << 8));
                var rightForeground = (ushort)(data[index++] | (data[index++] << 8));
                var tile = new Tile(background, leftForeground, rightForeground);

                mapTemplate.Tiles[x, y] = tile;

                if (CHAOS_CONSTANTS.DOOR_SPRITES.Contains(leftForeground))
                    mapTemplate.Doors[point] = new DoorTemplate
                    {
                        Sprite = leftForeground,
                        OpenRight = true,
                        Closed = true,
                        Point = point
                    };
                else if (CHAOS_CONSTANTS.DOOR_SPRITES.Contains(rightForeground))
                    mapTemplate.Doors[point] = new DoorTemplate
                    {
                        Sprite = rightForeground,
                        OpenRight = false,
                        Closed = true,
                        Point = point
                    };
            }

        mapTemplate.CheckSum = data.Generate16();
    }

    private async Task LoadTemplateAsync(string path)
    {
        try
        {
            var mapTemplate = await LoadTemplateFromFileAsync(path);
            
            Cache.TryAdd(mapTemplate.TemplateKey, mapTemplate);
            Logger.LogTrace("Loaded map template {MapId}", mapTemplate.TemplateKey);
        } catch (FileNotFoundException e)
        {
            var mapDataFileName = Path.GetFileName(e.FileName);
            var templateFileName = Path.GetFileName(path);
            File.Move(path, Path.Combine(NeedsMapDataDir, templateFileName));
            Logger.LogError("Map data not found for {FileName}", mapDataFileName);
        }
    }

    private async Task<MapTemplate> LoadTemplateFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var schema = await JsonSerializer.DeserializeAsync<MapTemplateSchema>(stream, JsonSerializerOptions);
        var mapTemplate = Mapper.Map<MapTemplate>(schema!);
        
        await LoadMapDataAsync(mapTemplate);

        return mapTemplate;
    }
}