using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class ItemTemplateCache : SimpleFileCacheBase<ItemTemplate, ItemTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<ItemTemplate, string> KeySelector { get; } = t => t.TemplateKey;

    /// <inheritdoc />
    public ItemTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ItemTemplateCacheOptions> options,
        ILogger<ItemTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringItemTemplateCache : ExpiringFileCacheBase<ItemTemplate, ItemTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringItemTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringItemTemplateCacheOptions> options,
        ILogger<ExpiringItemTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}