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

public sealed class MetafileCache : SimpleFileCacheBase<Metafile, MetafileSchema>
{
    /// <inheritdoc />
    protected override Func<Metafile, string> KeySelector => m => m.Name;

    /// <inheritdoc />
    public MetafileCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<MetafileCacheOptions> options,
        ILogger<MetafileCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringMetafileCache : ExpiringFileCacheBase<Metafile, MetafileSchema>
{
    /// <inheritdoc />
    public ExpiringMetafileCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringMetafileCacheOptions> options,
        ILogger<ExpiringMetafileCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}