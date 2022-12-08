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

public sealed class LootTableCache : SimpleFileCacheBase<LootTable, LootTableSchema>
{
    /// <inheritdoc />
    protected override Func<LootTable, string> KeySelector => l => l.Key;

    /// <inheritdoc />
    public LootTableCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<LootTableCacheOptions> options,
        ILogger<LootTableCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) =>
        AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringLootTableCache : ExpiringFileCacheBase<LootTable, LootTableSchema>
{
    /// <inheritdoc />
    public ExpiringLootTableCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringLootTableCacheOptions> options,
        ILogger<ExpiringLootTableCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}