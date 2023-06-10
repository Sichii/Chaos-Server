using System.IO;
using System.Text.Json;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class SpellTemplateRepository : RepositoryBase<SpellTemplateSchema>
{
    /// <inheritdoc />
    public SpellTemplateRepository(IOptions<SpellTemplateCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    public override void Add(string path, SpellTemplateSchema obj)
    {
        var wrapper = new TraceWrapper<SpellTemplateSchema>(path, obj);
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