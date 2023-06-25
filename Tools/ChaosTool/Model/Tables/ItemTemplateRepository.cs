using System.IO;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class ItemTemplateRepository : RepositoryBase<ItemTemplateSchema>
{
    /// <inheritdoc />
    public ItemTemplateRepository(IEntityRepository entityRepository, IOptions<ItemTemplateCacheOptions> options)
        : base(entityRepository, options) { }

    public override void Add(string path, ItemTemplateSchema obj)
    {
        var wrapper = new TraceWrapper<ItemTemplateSchema>(path, obj);

        Objects.Add(wrapper);
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Object.TemplateKey.EqualsI(name));

        if (wrapper is null)
            return;

        File.Delete(wrapper.Path);
        Objects.Remove(wrapper);
    }
}