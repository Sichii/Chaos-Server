using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

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