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

public sealed class ItemTemplateCache : SimpleFileCacheBase<ItemTemplate, ItemTemplateSchema, ItemTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<ItemTemplate, string> KeySelector { get; } = t => t.TemplateKey;

    /// <inheritdoc />
    public ItemTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ItemTemplateCacheOptions> options,
        ILogger<ItemTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class ExpiringItemTemplateCache : ExpiringFileCacheBase<ItemTemplate, ItemTemplateSchema, ExpiringItemTemplateCacheOptions>
{
    /// <inheritdoc />
    public ExpiringItemTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringItemTemplateCacheOptions> options,
        ILogger<ExpiringItemTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}