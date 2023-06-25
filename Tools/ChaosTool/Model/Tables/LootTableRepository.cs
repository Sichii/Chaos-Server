using System.IO;
using Chaos.Extensions.Common;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class LootTableRepository : RepositoryBase<LootTableSchema>
{
    /// <inheritdoc />
    public LootTableRepository(IEntityRepository entityRepository, IOptions<LootTableCacheOptions> options)
        : base(entityRepository, options) { }

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