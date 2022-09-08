using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class SpellTemplateCache : SimpleFileCacheBase<SpellTemplate, SpellTemplateSchema, SpellTemplateCacheOptions>
{
    /// <inheritdoc />
    public SpellTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SpellTemplateCacheOptions> options,
        ILogger<SpellTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(LoadCacheAsync);

    /// <inheritdoc />
    protected override Func<SpellTemplate, string> KeySelector => t => t.TemplateKey;
}