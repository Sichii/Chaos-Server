using Chaos.Cryptography;
using Chaos.Definitions;
using Chaos.Models.Map;
using Chaos.Models.Templates;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class ExpiringMapTemplateCache : ExpiringFileCache<MapTemplate, MapTemplateSchema, MapTemplateCacheOptions>
{
    private readonly string NeedsMapDataDir;
    private new MapTemplateCacheOptions Options { get; }

    /// <inheritdoc />
    public ExpiringMapTemplateCache(
        IMemoryCache cache,
        IEntityRepository entityRepository,
        IOptions<MapTemplateCacheOptions> options,
        ILogger<ExpiringMapTemplateCache> logger)
        : base(
            cache,
            entityRepository,
            options,
            logger)
    {
        Options = options.Value;
        NeedsMapDataDir = Path.Combine(Options.Directory, "NeedsMapData");

        if (!Directory.Exists(NeedsMapDataDir))
            Directory.CreateDirectory(NeedsMapDataDir);
    }

    protected override MapTemplate CreateFromEntry(ICacheEntry entry)
    {
        var key = entry.Key.ToString();
        var keyActual = DeconstructKeyForType(key!);

        Logger.WithTopics(Topics.Entities.MapTemplate, Topics.Actions.Load)
              .LogDebug("Creating new {@TypeName} entry with key {@Key}", nameof(MapTemplate), key);

        var metricsLogger = Logger.WithTopics(Topics.Entities.MapTemplate, Topics.Actions.Load)
                                  .WithMetrics();

        if (Options.Expires)
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins!.Value));

        entry.RegisterPostEvictionCallback(RemoveValueCallback);

        var path = GetPathForKey(keyActual);

        var ret = EntityRepository.LoadAndMap<MapTemplate, MapTemplateSchema>(path);

        LoadMapData(ret);

        LocalLookup[key!] = ret;

        metricsLogger.LogDebug("Created new {@TypeName} entry with key {@Key}", nameof(MapTemplate), key);

        return ret;
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

                if (CHAOS_CONSTANTS.DOOR_TILE_IDS.Contains(leftForeground))
                    mapTemplate.Doors[point] = new DoorTemplate
                    {
                        Sprite = leftForeground,
                        OpenRight = true,
                        Closed = true,
                        Point = point
                    };
                else if (CHAOS_CONSTANTS.DOOR_TILE_IDS.Contains(rightForeground))
                    mapTemplate.Doors[point] = new DoorTemplate
                    {
                        Sprite = rightForeground,
                        OpenRight = false,
                        Closed = true,
                        Point = point
                    };
            }

        mapTemplate.CheckSum = Crc.Generate16(data);
    }
}