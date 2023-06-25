using System.IO;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class MapTemplateRepository : RepositoryBase<MapTemplateSchema>
{
    /// <inheritdoc />
    public MapTemplateRepository(IEntityRepository entityRepository, IOptions<MapTemplateCacheOptions> options)
        : base(entityRepository, options) { }

    public override void Add(string path, MapTemplateSchema obj)
    {
        var wrapper = new TraceWrapper<MapTemplateSchema>(path, obj);
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