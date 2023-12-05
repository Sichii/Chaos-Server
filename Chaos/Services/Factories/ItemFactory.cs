using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class ItemFactory : IItemFactory
{
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public ItemFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider)
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
    }

    public Item Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.Get<ItemTemplate>(templateKey);
        var item = new Item(template, ScriptProvider, extraScriptKeys);

        return item;
    }

    /// <inheritdoc />
    public Item CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.Get<ItemTemplate>(templateKey);

        //creates an item with a unique id of 0
        var item = new Item(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        //no need to log creation of faux items

        return item;
    }
}