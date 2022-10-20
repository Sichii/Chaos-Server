using System.Text.Json;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class WorldMapCache : SimpleFileCacheBase<WorldMap, WorldMapSchema, WorldMapCacheOptions>
{
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

    /// <inheritdoc />
    protected override Func<WorldMap, string> KeySelector { get; } = wm => wm.WorldMapKey;
}