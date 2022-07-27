using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Serialization;

public class ItemSerialTransformService : ISerialTransformService<Item, SerializableItem>
{
    private readonly ISimpleCache<ItemTemplate> ItemTemplateCache;
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ILogger<ItemSerialTransformService> Logger;

    public ItemSerialTransformService(
        ISimpleCache<ItemTemplate> itemTemplateCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<ItemSerialTransformService> logger
    )
    {
        ItemTemplateCache = itemTemplateCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;
    }

    public Item Transform(SerializableItem serialized)
    {
        var item = new Item(serialized, ItemTemplateCache, ItemScriptFactory);

        Logger.LogTrace("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.Template.Name, item.UniqueId);

        return item;
    }

    public SerializableItem Transform(Item entity)
    {
        var ret = new SerializableItem(entity);

        Logger.LogTrace("Serialized item - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}