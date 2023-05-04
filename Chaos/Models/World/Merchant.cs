using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Models.World;

public sealed class Merchant : Creature,
                               IScripted<IMerchantScript>,
                               IBuyShopSource,
                               ISellShopSource,
                               ISkillTeacherSource,
                               ISpellTeacherSource
{
    /// <inheritdoc />
    public override int AssailIntervalMs => 500;
    public override bool IsAlive => true;

    /// <inheritdoc />
    public ICollection<Item> ItemsForSale { get; }

    /// <inheritdoc />
    public ICollection<string> ItemsToBuy { get; }
    public override ILogger<Merchant> Logger { get; }
    /// <inheritdoc />
    public override IMerchantScript Script { get; }
    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }

    /// <inheritdoc />
    public ICollection<Skill> SkillsToTeach { get; }

    /// <inheritdoc />
    public ICollection<Spell> SpellsToTeach { get; }
    public override StatSheet StatSheet { get; }
    public IStockService StockService { get; }
    public MerchantTemplate Template { get; }

    public override CreatureType Type { get; }

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => DisplayColor.Default;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Creature;

    public Merchant(
        MerchantTemplate template,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Merchant> logger,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        IItemFactory itemFactory,
        IStockService stockService,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null
    )
        : base(
            template.Name,
            template.Sprite,
            mapInstance,
            point)
    {
        extraScriptKeys ??= Array.Empty<string>();

        Template = template;
        Logger = logger;
        StatSheet = StatSheet.Maxed;
        Type = CreatureType.Merchant;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, this);
        StockService = stockService;

        ItemsForSale = template.ItemsForSale.Select(
                                   kvp =>
                                   {
                                       var item = itemFactory.CreateFaux(kvp.Key);

                                       return item;
                                   })
                               .ToList();

        ItemsToBuy = template.ItemsToBuy.ToList();
        SkillsToTeach = template.SkillsToTeach.Select(key => skillFactory.CreateFaux(key)).ToList();
        SpellsToTeach = template.SpellsToTeach.Select(key => spellFactory.CreateFaux(key)).ToList();

        //register this merchant's stock with the stock service
        if (Template.ItemsForSale.Any())
            StockService.RegisterStock(
                Template.TemplateKey,
                Template.ItemsForSale.Select(kvp => (kvp.Key, kvp.Value)),
                TimeSpan.FromHours(Template.RestockIntervalHours),
                Template.RestockPercent);
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    int IBuyShopSource.GetStock(string itemTemplateKey) => StockService.GetStock(Template.TemplateKey, itemTemplateKey);

    /// <inheritdoc />
    bool IBuyShopSource.HasStock(string itemTemplateKey) => StockService.HasStock(Template.TemplateKey, itemTemplateKey);

    /// <inheritdoc />
    public bool IsBuying(Item item) => ItemsToBuy.Contains(item.Template.TemplateKey);

    /// <inheritdoc />
    public override void OnApproached(Creature creature)
    {
        base.OnApproached(creature);

        Script.OnApproached(creature);
    }

    public override void OnClicked(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    public override void OnDeparture(Creature creature)
    {
        base.OnDeparture(creature);

        Script.OnDeparture(creature);
    }

    public override void OnGoldDroppedOn(Aisling source, int amount) => Script.OnGoldDroppedOn(source, amount);

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count) => Script.OnItemDroppedOn(source, slot, count);

    /// <inheritdoc />
    void IBuyShopSource.Restock(decimal percent) => StockService.Restock(Template.TemplateKey, percent);

    /// <inheritdoc />
    bool IBuyShopSource.TryDecrementStock(string itemTemplateKey, int amount) =>
        StockService.TryDecrementStock(Template.TemplateKey, itemTemplateKey, amount);

    /// <inheritdoc />
    public bool TryGetItem(string itemName, [MaybeNullWhen(false)] out Item item)
    {
        item = ItemsForSale.FirstOrDefault(item => item.DisplayName.EqualsI(itemName));

        return item != null;
    }

    /// <inheritdoc />
    public bool TryGetSkill(string skillName, [MaybeNullWhen(false)] out Skill skill)
    {
        skill = SkillsToTeach.FirstOrDefault(skill => skill.Template.Name.EqualsI(skillName));

        return skill != null;
    }

    /// <inheritdoc />
    public bool TryGetSpell(string spellName, [MaybeNullWhen(false)] out Spell spell)
    {
        spell = SpellsToTeach.FirstOrDefault(spell => spell.Template.Name.EqualsI(spellName));

        return spell != null;
    }
}