using System.IO;
using System.Text.Json;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class MonsterTemplateRepository : RepositoryBase<MonsterTemplateSchema>
{
    /// <inheritdoc />
    public MonsterTemplateRepository(IOptions<MonsterTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    public override void Add(string path, MonsterTemplateSchema obj)
    {
        var wrapper = new TraceWrapper<MonsterTemplateSchema>(path, obj);
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