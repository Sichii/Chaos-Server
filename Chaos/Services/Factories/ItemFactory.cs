using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class ItemFactory : IItemFactory
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ISimpleCache<ItemTemplate> ItemTemplateCache;
    private readonly ILogger Logger;

    public ItemFactory(
        ISimpleCache<ItemTemplate> itemTemplateCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<ItemFactory> logger
    )
    {
        ItemTemplateCache = itemTemplateCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;
    }

    public Item Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = ItemTemplateCache.GetObject(templateKey);
        var item = new Item(template, ItemScriptFactory, extraScriptKeys);

        Logger.LogDebug("Created item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }

    public Item Deserialize(SerializableItem serialized)
    {
        var item = new Item(serialized, ItemTemplateCache, ItemScriptFactory);

        Logger.LogDebug("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }
}