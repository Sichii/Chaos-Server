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

public class MerchantTemplateCache : SimpleFileCacheBase<MerchantTemplate, MerchantTemplateSchema, MerchantTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<MerchantTemplate, string> KeySelector { get; } = m => m.TemplateKey;

    /// <inheritdoc />
    public MerchantTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MerchantTemplateCacheOptions> options,
        ILogger<MerchantTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}