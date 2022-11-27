using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class DialogTemplateCache : SimpleFileCacheBase<DialogTemplate, DialogTemplateSchema, DialogTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<DialogTemplate, string> KeySelector { get; } = d => d.TemplateKey;

    /// <inheritdoc />
    public DialogTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<DialogTemplateCacheOptions> options,
        ILogger<DialogTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}