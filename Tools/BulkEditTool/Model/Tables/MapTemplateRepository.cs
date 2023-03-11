using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class MapTemplateRepository : RepositoryBase<MapTemplateSchema, MapTemplateCacheOptions>
{
    /// <inheritdoc />
    public MapTemplateRepository(IOptions<MapTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }
}