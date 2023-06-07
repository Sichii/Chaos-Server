using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class LootTableRepository : RepositoryBase<LootTableSchema, LootTableCacheOptions>
{
    /// <inheritdoc />
    public LootTableRepository(IOptions<LootTableCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    public override void Add(string path, LootTableSchema obj)
    {
        var wrapper = new TraceWrapper<LootTableSchema>(path, obj);
        Objects.Add(wrapper);
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Object.Key.EqualsI(name));

        if (wrapper is null)
            return;

        File.Delete(wrapper.Path);
        Objects.Remove(wrapper);
    }
}