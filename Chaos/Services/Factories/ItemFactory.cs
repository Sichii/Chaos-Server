using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class ItemFactory : IItemFactory
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ILogger Logger;
    private readonly ISimpleCache SimpleCache;

    public ItemFactory(
        ISimpleCache simpleCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<ItemFactory> logger
    )
    {
        SimpleCache = simpleCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;
    }

    public Item Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.GetObject<ItemTemplate>(templateKey);
        var item = new Item(template, ItemScriptFactory, extraScriptKeys);

        Logger.LogDebug("Created item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }

    public Item Deserialize(ItemSchema schema)
    {
        var item = new Item(schema, SimpleCache, ItemScriptFactory);

        Logger.LogDebug("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.DisplayName, item.UniqueId);

        return item;
    }
}