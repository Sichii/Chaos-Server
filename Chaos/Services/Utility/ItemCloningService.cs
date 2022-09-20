using Chaos.Objects.Panel;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Services.Utility.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Utility;

public class ItemCloningService : ICloningService<Item>
{
    private readonly ILogger<ItemCloningService> Logger;
    private readonly IScriptProvider ScriptProvider;

    public ItemCloningService(IScriptProvider scriptProvider, ILogger<ItemCloningService> logger)
    {
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Item Clone(Item obj)
    {
        var cloned = new Item(obj.Template, ScriptProvider, obj.ScriptKeys)
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