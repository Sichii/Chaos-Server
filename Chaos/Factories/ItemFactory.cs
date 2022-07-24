using Chaos.Caches;
using Chaos.Caches.Interfaces;
using Chaos.Core.Identity;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

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

    public Item CreateItem(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = ItemTemplateCache.GetObject(templateKey);
        var item = new Item(template, ItemScriptFactory, extraScriptKeys);

        Logger.LogDebug("Created item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }

    public Item DeserializeItem(SerializableItem serializableItem)
    {
        var item = new Item(serializableItem, ItemTemplateCache, ItemScriptFactory);

        Logger.LogDebug("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);
        
        return item;
    }

    public Item CloneItem(Item item)
    {
        var cloned = new Item(item.Template, ItemScriptFactory, item.ScriptKeys)
        {
            Color = item.Color,
            Count = item.Count,
            CurrentDurability = item.CurrentDurability,
            Slot = item.Slot
        };

        Logger.LogDebug(
            "Cloned item - Name: {ItemName} UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            item.DisplayName,
            item.UniqueId,
            cloned.UniqueId);
        
        return cloned;
    }
}