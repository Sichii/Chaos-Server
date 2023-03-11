using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class SkillTemplateRepository : RepositoryBase<SkillTemplateSchema, SkillTemplateCacheOptions>
{
    /// <inheritdoc />
    public SkillTemplateRepository(IOptions<SkillTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }
}