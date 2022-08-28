using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Mappers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Mappers;

public class ItemTypeMapper : ITypeMapper<Item, ItemSchema>,
                              ITypeMapper<Item, ItemInfo>
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ILogger<ItemTypeMapper> Logger;
    private readonly ISimpleCache SimpleCache;

    public ItemTypeMapper(
        ISimpleCache simpleCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<ItemTypeMapper> logger
    )
    {
        SimpleCache = simpleCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;
    }

    public Item Map(ItemSchema obj)
    {
        var item = new Item(obj, SimpleCache, ItemScriptFactory);

        Logger.LogTrace("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.Template.Name, item.UniqueId);

        return item;
    }

    public Item Map(ItemInfo obj) => throw new NotImplementedException();
    ItemInfo ITypeMapper<Item, ItemInfo>.Map(Item obj) => new()
    {
        Class = obj.Template.BaseClass ?? BaseClass.Peasant,
        Color = obj.Color,
        Cost = obj.Template.Value,
        Count = obj.Count < 0
            ? throw new InvalidOperationException($"Item \"{obj.DisplayName}\" has negative count of {obj.Count}")
            : Convert.ToUInt32(obj.Count),
        CurrentDurability = obj.CurrentDurability ?? 0,
        GameObjectType = GameObjectType.Item,
        MaxDurability = obj.Template.MaxDurability ?? 0,
        Name = obj.DisplayName,
        Slot = obj.Slot,
        Sprite = obj.Template.ItemSprite.PanelSprite,
        Stackable = obj.Template.Stackable
    };

    public ItemSchema Map(Item obj)
    {
        var ret = new ItemSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase),
            TemplateKey = obj.Template.TemplateKey,
            Color = obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot
        };

        Logger.LogTrace("Serialized item - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}