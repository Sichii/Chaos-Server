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

public class MonsterTemplateCache : SimpleFileCacheBase<MonsterTemplate, MonsterTemplateSchema, MonsterTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<MonsterTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public MonsterTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MonsterTemplateCacheOptions> options,
        ILogger<MonsterTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}