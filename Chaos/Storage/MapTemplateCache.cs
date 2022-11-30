using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Cryptography;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class MapTemplateCache : SimpleFileCacheBase<MapTemplate, MapTemplateSchema, MapTemplateCacheOptions>
{
    private readonly string NeedsMapDataDir;

    /// <inheritdoc />
    protected override Func<MapTemplate, string> KeySelector => t => t.TemplateKey;

    public MapTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MapTemplateCacheOptions> options,
        ILogger<MapTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger)
    {
        NeedsMapDataDir = Path.Combine(Options.Directory, "NeedsMapData");

        if (!Directory.Exists(NeedsMapDataDir))
            Directory.CreateDirectory(NeedsMapDataDir);

        AsyncHelpers.RunSync(ReloadAsync);
    }

    private async Task<MapTemplate> InnerLoadFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var schema = await JsonSerializer.DeserializeAsync<MapTemplateSchema>(stream, JsonSerializerOptions);
        var mapTemplate = Mapper.Map<MapTemplate>(schema!);

        await LoadMapDataAsync(mapTemplate);

        return mapTemplate;
    }

    protected override async Task<MapTemplate?> LoadFromFileAsync(string path)
    {
        try
        {
            return await InnerLoadFromFileAsync(path);
        } catch (FileNotFoundException e)
        {
            var mapDataFileName = Path.GetFileName(e.FileName);
            var templateFileName = Path.GetFileName(path);
            File.Move(path, Path.Combine(NeedsMapDataDir, templateFileName));
            Logger.LogError("Map data not found for {FileName}", mapDataFileName);

            return null;
        }
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
}

public sealed class ExpiringMapTemplateCache : ExpiringFileCacheBase<MapTemplate, MapTemplateSchema, ExpiringMapTemplateCacheOptions>
{
    private readonly string NeedsMapDataDir;

    /// <inheritdoc />
    public ExpiringMapTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringMapTemplateCacheOptions> options,
        ILogger<ExpiringMapTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger)
    {
        NeedsMapDataDir = Path.Combine(Options.Directory, "NeedsMapData");

        if (!Directory.Exists(NeedsMapDataDir))
            Directory.CreateDirectory(NeedsMapDataDir);
    }

    protected override MapTemplate LoadFromFile(string path)
    {
        using var stream = File.OpenRead(path);
        var schema = JsonSerializer.Deserialize<MapTemplateSchema>(stream, JsonSerializerOptions);

        if (schema == null)
            throw new SerializationException($"Failed to serialize {nameof(MapTemplateSchema)} from path \"{path}\"");

        var mapTemplate = Mapper.Map<MapTemplate>(schema!);

        LoadMapData(mapTemplate);

        return mapTemplate;
    }

    private void LoadMapData(MapTemplate mapTemplate)
    {
        var path = Path.Combine(Options.MapDataDirectory, $"lod{mapTemplate.TemplateKey}.map");
        var data = File.ReadAllBytes(path);
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
}