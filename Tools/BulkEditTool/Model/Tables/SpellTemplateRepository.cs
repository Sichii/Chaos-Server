using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class SpellTemplateRepository : RepositoryBase<SpellTemplateSchema, SpellTemplateCacheOptions>
{
    /// <inheritdoc />
    public SpellTemplateRepository(IOptions<SpellTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }
}