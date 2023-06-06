using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class ItemTemplateRepository : RepositoryBase<ItemTemplateSchema, ItemTemplateCacheOptions>
{
    /// <inheritdoc />
    public ItemTemplateRepository(IOptions<ItemTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    public override void Add(string path, ItemTemplateSchema obj)
    {
        var wrapper = new TraceWrapper<ItemTemplateSchema>(path, obj);

        Objects.Add(wrapper);
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Obj.TemplateKey.EqualsI(name));

        if (wrapper is null)
            return;

        File.Delete(wrapper.Path);
        Objects.Remove(wrapper);
    }
}