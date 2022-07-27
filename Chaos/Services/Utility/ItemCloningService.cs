using Chaos.Objects.Panel;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Utility.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Utility;

public class ItemCloningService : ICloningService<Item>
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ILogger<ItemCloningService> Logger;

    public ItemCloningService(IItemScriptFactory itemScriptFactory, ILogger<ItemCloningService> logger)
    {
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;
    }

    public Item Clone(Item obj)
    {
        var cloned = new Item(obj.Template, ItemScriptFactory, obj.ScriptKeys)
        {
            Color = obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot
        };

        Logger.LogDebug(
            "Cloned item - Name: {ItemName} UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            obj.DisplayName,
            obj.UniqueId,
            cloned.UniqueId);

        return cloned;
    }
}