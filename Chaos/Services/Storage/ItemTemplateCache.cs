using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class ItemTemplateCache : SimpleFileCacheBase<ItemTemplate, ItemTemplateSchema, ItemTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<ItemTemplate, string> KeySelector => t => t.TemplateKey;

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