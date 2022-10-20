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

public sealed class LootTableCache : SimpleFileCacheBase<LootTable, LootTableSchema, LootTableCacheOptions>
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