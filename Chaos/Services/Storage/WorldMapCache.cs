using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class WorldMapCache : SimpleFileCacheBase<WorldMap, WorldMapSchema>
{
    /// <inheritdoc />
    protected override Func<WorldMap, string> KeySelector { get; } = wm => wm.WorldMapKey;

    /// <inheritdoc />
    public WorldMapCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<WorldMapCacheOptions> options,
        ILogger<WorldMapCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringWorldMapCache : ExpiringFileCacheBase<WorldMap, WorldMapSchema>
{
    /// <inheritdoc />
    public ExpiringWorldMapCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringWorldMapCacheOptions> options,
        ILogger<ExpiringWorldMapCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}