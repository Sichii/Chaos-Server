#region
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Utilities;
using Humanizer;
#endregion

namespace Chaos.Scripting.MerchantScripts.ShopScripts.Abstractions;

public abstract class VerbalShopScriptBase : MerchantScriptBase
{
    protected static ICollection<string> AllTerms { get; } = new List<string>
    {
        "all",
        "everything"
    };

    protected static ICollection<string> BuyItemPhrases { get; } = new List<string>
    {
        "{Name}, we have a deal! {AmountOfThing} is yours.",
        "A fine trade, {Name}. Enjoy your {AmountOfThing}.",
        "It's a deal, {Name}. Here's your {AmountOfThing}.",
        "Bargain struck, {Name}. You now have {AmountOfThing}.",
        "Agreed, {Name}. {AmountOfThing} is now yours."
    };

    protected static ICollection<string> CantCarryPhrases { get; } = new List<string>
    {
        "{Name}, you can't carry {AmountOfThing}.",
        "Sorry, {Name}. You don't seem to be able to carry {AmountOfThing}.",
        "{Name}, it seems you can't handle {AmountOfThing}.",
        "{Name}, it looks like {AmountOfThing} is too much for you.",
        "{Name}, it appears you can't take {AmountOfThing}."
    };

    protected static ICollection<string> DontHaveThatManySellPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {AmountOfThing} to sell.",
        "{Name}, {AmountOfThing} seems to exceed your inventory.",
        "{Name}, you don't have {AmountOfThing} on hand.",
        "{Name}, you're missing {AmountOfThing} for sale.",
        "{Name}, can't find your {AmountOfThing} for sale."
    };

    protected static ICollection<string> ItemDamagedSellPhrases { get; } = new List<string>
    {
        "{Name}, I dont want your {AmountOfThing}, ask a smith to fix it.",
        "Sorry, {Name}. Your {AmountOfThing} won't sell in this condition.",
        "{Name}, I can't accept your damaged {AmountOfThing}.",
        "This isn't a charity, {Name}, I don't want your {AmountOfThing}.",
        "Sorry, {Name}, I can't take your {AmountOfThing} in this condition."
    };

    protected ILogger Logger { get; }

    protected static ICollection<string> NotEnoughGoldPhrases { get; } = new List<string>
    {
        "{Name}, you can't afford {AmountOfThing}.",
        "Sorry, {Name}. {AmountOfThing} is too pricey for you.",
        "{Name}, you need more gold for {AmountOfThing}.",
        "{Name}, your gold isn't enough for {AmountOfThing}.",
        "{Name}, you're short on gold for {AmountOfThing}."
    };

    protected static ICollection<string> OutOfStockPhrases { get; } = new List<string>
    {
        "Sorry, {Name}. I don't have {AmountOfThing} in stock.",
        "{Name}, I can't provide {AmountOfThing}. Stock is low.",
        "{Name}, I don't have {AmountOfThing} available.",
        "{Name}, I can't fulfill {AmountOfThing}. We're short.",
        "{Name}, we're short of stock. Can't get you {AmountOfThing}."
    };

    protected static ICollection<string> SellItemPhrases { get; } = new List<string>
    {
        "{Name}, I'll buy {AmountOfThing} for {Gold} gold.",
        "Deal, {Name}. Your {AmountOfThing} for {Gold} gold.",
        "{Name}, we have a deal. {AmountOfThing} for {Gold} gold.",
        "Agreed, {Name}. I'll take {AmountOfThing} for my {Gold} gold.",
        "{Name}, it's a deal. Your {AmountOfThing} for my {Gold} gold."
    };

    /// <inheritdoc />
    protected VerbalShopScriptBase(Merchant subject, ILogger logger)
        : base(subject)
        => Logger = logger;

    protected virtual string Humanize(
        string template,
        string name,
        int amount,
        string thing,
        int? gold = null)
    {
        if (amount > 1)
            template.ReplaceI("{AmountOfThing} is", "{AmountOfThing} are");
        else
            template.ReplaceI("{AmountOfThing} are", "{AmountOfThing} is");

        if (gold.HasValue)
            return template.Inject(name, thing.ToQuantity(amount), gold.Value);

        return template.Inject(name, thing.ToQuantity(amount));
    }

    protected virtual bool IsClosestVerbalShopTo(Aisling aisling)
    {
        //if we're checking, it means we're in range... it should be impossible to get no results
        var closestVerbalShop = aisling.MapInstance
                                       .GetEntities<Merchant>()
                                       .Where(merchant => merchant.Script.Is<VerbalShopScriptBase>())
                                       .OrderBy(x => x.ManhattanDistanceFrom(aisling))
                                       .ThenBy(x => x.Creation)
                                       .First();

        if (closestVerbalShop.Equals(Subject))
            return true;

        return false;
    }

    protected virtual void RandomizedReply(
        Merchant source,
        ICollection<string> phrases,
        string name,
        int amount,
        string thing,
        int? goldAmount = null)
    {
        var phrase = phrases.PickRandom();

        phrase = Humanize(
            phrase,
            name,
            amount,
            thing,
            goldAmount);

        source.Say(phrase);
    }

    protected virtual void ReplyToUnknownInput(Aisling aisling) => Subject.Say(DialogString.UnknownInput);
}