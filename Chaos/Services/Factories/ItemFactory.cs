using Chaos.Objects.Panel;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class ItemFactory : IItemFactory
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
        var template = SimpleCache.GetObject<ItemTemplate>(templateKey);
        var item = new Item(template, ScriptProvider, extraScriptKeys);

        Logger.LogDebug("Created item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }
}