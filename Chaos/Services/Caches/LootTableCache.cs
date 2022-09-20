using System.Text.Json;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Content;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class LootTableCache : SimpleFileCacheBase<LootTable, LootTableSchema, LootTableCacheOptions>
{
    /// <inheritdoc />
    protected override Func<LootTable, string> KeySelector => l => l.Key;

    /// <inheritdoc />
    public LootTableCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<LootTableCacheOptions> options,
        ILogger<LootTableCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) =>
        AsyncHelpers.RunSync(ReloadAsync);
}