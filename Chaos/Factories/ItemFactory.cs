using Chaos.Caches.Interfaces;
using Chaos.Core.Identity;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class ItemFactory : IItemFactory
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ISimpleCache<string, ItemTemplate> ItemTemplateCache;
    private readonly ILogger Logger;

    public ItemFactory(
        ISimpleCache<string, ItemTemplate> itemTemplateCache,
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
        var item = new Item(template);

        var scriptKeys = template.ScriptKeys
                                 .Concat(extraScriptKeys)
                                 .ToHashSet(StringComparer.OrdinalIgnoreCase);

        item.Script = ItemScriptFactory.CreateScript(scriptKeys, item);
        item.UniqueId = ServerId.NextId;
        Logger.LogDebug("Created item \"{ItemName}\" with unique id \"{UniqueId}\"", item.DisplayName, item.UniqueId);

        return item;
    }
}