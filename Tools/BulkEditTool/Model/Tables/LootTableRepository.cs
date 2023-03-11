using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class LootTableRepository : RepositoryBase<LootTableSchema, LootTableCacheOptions>
{
    /// <inheritdoc />
    public LootTableRepository(IOptions<LootTableCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }
}