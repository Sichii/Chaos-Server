using Chaos.Objects.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class ItemFactory : IItemFactory
{
    private readonly ILogger Logger;
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public ItemFactory(
        ISimpleCache simpleCache,
        ITypeMapper mapper,
        IScriptProvider scriptProvider,
        ILogger<ItemFactory> logger
    )
    {
        SimpleCache = simpleCache;
        Mapper = mapper;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Item Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.Get<ItemTemplate>(templateKey);
        var item = new Item(template, ScriptProvider, extraScriptKeys);

        Logger.LogTrace("Created item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }
}