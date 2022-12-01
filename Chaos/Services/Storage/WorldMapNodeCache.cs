using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class WorldMapNodeCache : SimpleFileCacheBase<WorldMapNode, WorldMapNodeSchema, WorldMapNodeCacheOptions>
{
    /// <inheritdoc />
    protected override Func<WorldMapNode, string> KeySelector { get; } = n => n.NodeKey;

    /// <inheritdoc />
    public WorldMapNodeCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<WorldMapNodeCacheOptions> options,
        ILogger<WorldMapNodeCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringWorldMapNodeCache : ExpiringFileCacheBase<WorldMapNode, WorldMapNodeSchema, ExpiringWorldMapNodeCacheOptions>
{
    /// <inheritdoc />
    public ExpiringWorldMapNodeCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringWorldMapNodeCacheOptions> options,
        ILogger<ExpiringWorldMapNodeCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}