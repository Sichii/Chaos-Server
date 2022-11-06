using System.Text.Json;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public class WorldMapCache : SimpleFileCacheBase<WorldMap, WorldMapSchema, WorldMapCacheOptions>
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