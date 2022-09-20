using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Entities.Schemas.Content;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MetafileCache : SimpleFileCacheBase<Metafile, MetafileSchema, MetafileCacheOptions>
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