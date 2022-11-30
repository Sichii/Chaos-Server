using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class MetafileCache : SimpleFileCacheBase<Metafile, MetafileSchema, MetafileCacheOptions>
{
    /// <inheritdoc />
    protected override Func<Metafile, string> KeySelector => m => m.Name;

    /// <inheritdoc />
    public MetafileCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MetafileCacheOptions> options,
        ILogger<MetafileCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringMetafileCache : ExpiringFileCacheBase<Metafile, MetafileSchema, ExpiringMetafileCacheOptions>
{
    /// <inheritdoc />
    public ExpiringMetafileCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringMetafileCacheOptions> options,
        ILogger<ExpiringMetafileCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}