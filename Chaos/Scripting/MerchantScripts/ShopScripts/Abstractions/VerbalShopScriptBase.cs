using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

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
        "{Name}, we have a deal! {Amount} {Thing} is yours.",
        "A fine trade, {Name}. Enjoy your {Amount} {Thing}.",
        "It's a deal, {Name}. Here's your {Amount} {Thing}.",
        "Bargain struck, {Name}. You now have {Amount} {Thing}.",
        "Agreed, {Name}. {Amount} {Thing} is now yours."
    };

    protected static ICollection<string> CantCarryPhrases { get; } = new List<string>
    {
        "{Name}, you can't carry {Amount} {Thing}.",
        "Sorry, {Name}. You don't seem to be able to carry {Amount} {Thing}.",
        "{Name}, it seems you can't handle {Amount} more {Thing}.",
        "{Name}, it looks like {Amount} {Thing} is too much for you.",
        "{Name}, it appears you can't take {Amount} more {Thing}."
    };

    protected static ICollection<string> DontHaveThatManySellPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {Amount} {Thing} to sell.",
        "{Name}, {Amount} {Thing} seems to exceed your inventory.",
        "{Name}, you don't have {Amount} {Thing} on hand.",
        "{Name}, you're missing {Amount} {Thing} for sale.",
        "{Name}, can't find your {Amount} {Thing} for sale."
    };
    protected ILogger Logger { get; }

    protected static ICollection<string> NotEnoughGoldPhrases { get; } = new List<string>
    {
        "{Name}, you can't afford {Amount} of {Thing}.",
        "Sorry, {Name}. {Amount} of {Thing} is too pricey for you.",
        "{Name}, you need more gold for {Amount} of {Thing}.",
        "{Name}, your gold isn't enough for {Amount} of {Thing}.",
        "{Name}, you're short on gold for {Amount} of {Thing}."
    };

    protected static ICollection<string> OutOfStockPhrases { get; } = new List<string>
    {
        "Sorry, {Name}. I don't have {Amount} {Thing} in stock.",
        "{Name}, I can't provide {Amount} {Thing}. Stock is low.",
        "{Name}, I don't have {Amount} {Thing} available.",
        "{Name}, I can't fulfill {Amount} {Thing}. We're short.",
        "{Name}, we're short of stock. Can't get you {Amount} {Thing}."
    };

    protected static ICollection<string> SellItemPhrases { get; } = new List<string>
    {
        "{Name}, I'll buy {Amount} {Thing} for {Gold} gold.",
        "Deal, {Name}. Your {Amount} {Thing} for {Gold} gold.",
        "{Name}, we have a deal. {Amount} {Thing} for {Gold} gold.",
        "Agreed, {Name}. I'll take {Amount} {Thing} for my {Gold} gold.",
        "{Name}, it's a deal. Your {Amount} {Thing} for my {Gold} gold."
    };

    /// <inheritdoc />
    protected VerbalShopScriptBase(Merchant subject, ILogger logger)
        : base(subject) => Logger = logger;

    protected virtual bool IsClosestVerbalShopTo(Aisling aisling)
    {
        //if we're checking, it means we're in range... it should be impossible to get no results
        var closestVerbalShop = aisling.MapInstance.GetEntities<Merchant>()
                                       .Where(merchant => merchant.Script.Is<VerbalShopScriptBase>())
                                       .OrderBy(x => x.DistanceFrom(aisling))
                                       .ThenBy(x => x.Creation)
                                       .First();

        if (closestVerbalShop.Equals(Subject))
            return true;

        return false;
    }

    protected virtual void ReplyToUnknownInput(Aisling aisling) => Subject.Say(DialogString.UnknownInput);
}