#region
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Models.World;

public sealed class Merchant : Creature,
                               IScripted<IMerchantScript>,
                               IBuyShopSource,
                               ISellShopSource,
                               ISkillTeacherSource,
                               ISpellTeacherSource
{
    public ICollection<IPoint> BlackList { get; set; }

    /// <inheritdoc />
    public ICollection<Item> ItemsForSale { get; }

    /// <inheritdoc />
    public ICollection<Item> ItemsToBuy { get; }

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
    public IIntervalTimer WanderTimer { get; }

    /// <inheritdoc />
    public override int AssailIntervalMs => 500;

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => DisplayColor.Default;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Creature;

    public override bool IsAlive => true;

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
        ICollection<string>? extraScriptKeys = null)
        : base(
            template.Name,
            template.Sprite,
            mapInstance,
            point)
    {
        extraScriptKeys ??= [];

        Template = template;
        Logger = logger;
        StatSheet = StatSheet.Maxed;
        Type = CreatureType.Merchant;
        StockService = stockService;
        WanderTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.WanderIntervalMs), 10, RandomizationType.Positive);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        BlackList = new HashSet<IPoint>(PointEqualityComparer.Instance);

        ItemsForSale = template.ItemsForSale
                               .Select(kvp =>
                               {
                                   var item = itemFactory.CreateFaux(kvp.Key);

                                   return item;
                               })
                               .ToList();

        ItemsToBuy = template.ItemsToBuy
                             .Select(itemKey => itemFactory.CreateFaux(itemKey))
                             .ToList();

        SkillsToTeach = template.SkillsToTeach
                                .Select(key => skillFactory.CreateFaux(key))
                                .ToList();

        SpellsToTeach = template.SpellsToTeach
                                .Select(key => spellFactory.CreateFaux(key))
                                .ToList();

        //register this merchant's stock with the stock service
        if (Template.ItemsForSale.Any())
            StockService.RegisterStock(
                Template.TemplateKey,
                Template.ItemsForSale.Select(kvp => (kvp.Key, kvp.Value)),
                TimeSpan.FromHours(Template.RestockIntervalHrs),
                Template.RestockPct);

        Script = scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, this);
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    int IBuyShopSource.GetStock(string itemTemplateKey) => StockService.GetStock(Template.TemplateKey, itemTemplateKey);

    /// <inheritdoc />
    bool IBuyShopSource.HasStock(string itemTemplateKey) => StockService.HasStock(Template.TemplateKey, itemTemplateKey);

    /// <inheritdoc />
    public bool IsBuying(Item item) => ItemsToBuy.Any(i => i.DisplayName.EqualsI(item.DisplayName));

    /// <inheritdoc />
    void IBuyShopSource.Restock(int percent) => StockService.Restock(Template.TemplateKey, percent);

    /// <inheritdoc />
    bool IBuyShopSource.TryDecrementStock(string itemTemplateKey, int amount)
        => StockService.TryDecrementStock(Template.TemplateKey, itemTemplateKey, amount);

    /// <inheritdoc />
    public bool TryGetItem(string itemName, [MaybeNullWhen(false)] out Item item)
    {
        item = ItemsForSale.FirstOrDefault(item => item.DisplayName.EqualsI(itemName));

        return item != null;
    }

    public override void OnClicked(Aisling source) => Script.OnClicked(source);

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (!this.WithinRange(source, WorldOptions.Instance.TradeRange))
            return;

        if (!Script.CanDropMoneyOn(source, amount))
        {
            source.SendActiveMessage("You can't do that right now");

            return;
        }

        Script.OnGoldDroppedOn(source, amount);
    }

    public bool TryGetSkill(string skillName, [MaybeNullWhen(false)] out Skill skill)
    {
        skill = SkillsToTeach.FirstOrDefault(skill => skill.Template.Name.EqualsI(skillName));

        return skill != null;
    }

    public bool TryGetSpell(string spellName, [MaybeNullWhen(false)] out Spell spell)
    {
        spell = SpellsToTeach.FirstOrDefault(spell => spell.Template.Name.EqualsI(spellName));

        return spell != null;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        WanderTimer.Update(delta);
        base.Update(delta);
    }

    /// <inheritdoc />
    public override void Wander(IPathOptions? pathOptions = null, bool ignoreCollision = false)
    {
        pathOptions ??= PathOptions.Default.ForCreatureType(Type);

        pathOptions.BlockedPoints = pathOptions.BlockedPoints
                                               .Concat(BlackList)
                                               .ToHashSet();

        base.Wander(pathOptions, ignoreCollision);
    }
}