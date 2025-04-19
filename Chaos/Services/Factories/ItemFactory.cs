#region
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Services.Factories;

public sealed class ItemFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider) : IItemFactory
{
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

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

    /// <inheritdoc />
    public Item CreateScriptProxy(ICollection<string>? extraScriptKeys = null)
    {
        var template = new ItemTemplate
        {
            AccountBound = false,
            BuyCost = 0,
            Category = "other",
            Color = DisplayColor.Default,
            EquipmentType = null,
            Gender = null,
            IsDyeable = false,
            IsModifiable = false,
            ItemSprite = new ItemSprite(0, 0),
            MaxDurability = null,
            MaxStacks = 0,
            Modifiers = null,
            NoTrade = false,
            PanelSprite = 0,
            PantsColor = null,
            SellValue = 0,
            Weight = 0,
            AbilityLevel = 0,
            AdvClass = null,
            Class = null,
            Cooldown = null,
            Description = null,
            Level = 0,
            Name = "Script Proxy",
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            TemplateKey = "scriptProxy"
        };

        var item = new Item(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        return item;
    }
}