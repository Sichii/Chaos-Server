using System.Text.Json;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class ExpiringReactorTileTemplateCache : ExpiringFileCacheBase<ReactorTileTemplate, ReactorTileTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringReactorTileTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringReactorTileTemplateCacheOptions> options,
        ILogger<ExpiringReactorTileTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}