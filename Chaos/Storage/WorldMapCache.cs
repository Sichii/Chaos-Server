using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class WorldMapCache : SimpleFileCacheBase<WorldMap, WorldMapSchema, WorldMapCacheOptions>
{
    /// <inheritdoc />
    protected override Func<WorldMap, string> KeySelector { get; } = wm => wm.WorldMapKey;

    /// <inheritdoc />
    public WorldMapCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<WorldMapCacheOptions> options,
        ILogger<WorldMapCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringWorldMapCache : ExpiringFileCacheBase<WorldMap, WorldMapSchema, ExpiringWorldMapCacheOptions>
{
    /// <inheritdoc />
    public ExpiringWorldMapCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringWorldMapCacheOptions> options,
        ILogger<ExpiringWorldMapCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}