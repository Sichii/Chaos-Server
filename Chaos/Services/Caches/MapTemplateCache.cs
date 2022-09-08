using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Core.Utilities;
using Chaos.Cryptography.Extensions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MapTemplateCache : SimpleFileCacheBase<MapTemplate, MapTemplateSchema, MapTemplateCacheOptions>
{
    private readonly string NeedsMapDataDir;

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

        AsyncHelpers.RunSync(LoadCacheAsync);
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

    /// <inheritdoc />
    protected override Func<MapTemplate, string> KeySelector => t => t.TemplateKey;

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

    private async Task<MapTemplate> InnerLoadFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var schema = await JsonSerializer.DeserializeAsync<MapTemplateSchema>(stream, JsonSerializerOptions);
        var mapTemplate = Mapper.Map<MapTemplate>(schema!);

        await LoadMapDataAsync(mapTemplate);

        return mapTemplate;
    }
}