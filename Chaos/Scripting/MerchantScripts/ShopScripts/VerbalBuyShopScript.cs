using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.ShopScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.MerchantScripts.ShopScripts;

public class VerbalBuyShopScript : VerbalShopScriptBase
{
    protected IBuyShopSource BuyShopSource { get; }
    protected ICloningService<Item> ItemCloner { get; }
    protected IItemFactory ItemFactory { get; }

    /// <inheritdoc />
    public VerbalBuyShopScript(
        Merchant subject,
        ILogger<VerbalBuyShopScript> logger,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner)
        : base(subject, logger)
    {
        if (subject is not IBuyShopSource buyShopSource)
            throw new ArgumentException("Merchant must implement IBuyShopSource", nameof(subject));

        ItemFactory = itemFactory;
        ItemCloner = itemCloner;
        BuyShopSource = buyShopSource;
    }

    protected virtual void BuyItem(Aisling source, Item itemToBuy, int amount)
    {
        var totalCost = itemToBuy.Template.BuyCost * amount;

        var result = ComplexActionHelper.BuyItem(
            source,
            BuyShopSource,
            itemToBuy,
            ItemFactory,
            ItemCloner,
            amount,
            itemToBuy.Template.BuyCost);

        switch (result)
        {
            case ComplexActionHelper.BuyItemResult.Success:
            {
                RandomizedReply(
                    Subject,
                    BuyItemPhrases,
                    source.Name,
                    amount,
                    itemToBuy.DisplayName);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Entities.Gold,
                          Topics.Actions.Buy)
                      .WithProperty(source)
                      .WithProperty(itemToBuy)
                      .WithProperty(BuyShopSource)
                      .LogInformation(
                          "Aisling {@AislingName} bought {ItemAmount} {@ItemName} from merchant {@MerchantName} for {GoldAmount} gold",
                          source.Name,
                          amount,
                          itemToBuy.DisplayName,
                          BuyShopSource.Name,
                          totalCost);

                break;
            }
            case ComplexActionHelper.BuyItemResult.CantCarry:
            {
                RandomizedReply(
                    Subject,
                    CantCarryPhrases,
                    source.Name,
                    amount,
                    itemToBuy.DisplayName);

                break;
            }
            case ComplexActionHelper.BuyItemResult.NotEnoughGold:
            {
                RandomizedReply(
                    Subject,
                    NotEnoughGoldPhrases,
                    source.Name,
                    amount,
                    itemToBuy.DisplayName);

                break;
            }
            case ComplexActionHelper.BuyItemResult.BadInput:
            {
                ReplyToUnknownInput(source);

                break;
            }
            case ComplexActionHelper.BuyItemResult.NotEnoughStock:
            {
                RandomizedReply(
                    Subject,
                    OutOfStockPhrases,
                    source.Name,
                    amount,
                    itemToBuy.DisplayName);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        //only listen to aislings
        if (source is not Aisling aisling)
            return;

        //make sure only the closest shop responds
        if (!IsClosestVerbalShopTo(aisling))
            return;

        var match = RegexCache.BUY_ITEM_PATTERNS
                              .Select(regex => regex.Match(message))
                              .FirstOrDefault(x => x.Success);

        if (match is null)
            return;

        var amountStr = "1";
        string thingstr;

        if (match.Groups.TryGetValue("amount", out var amountGroup) && amountGroup.Success)
            amountStr = amountGroup.Value;

        if (match.Groups.TryGetValue("thing", out var thingGroup) && thingGroup.Success)
            thingstr = thingGroup.Value;
        else
            return;

        var item = BuyShopSource.ItemsForSale.FuzzySearchBy(i => i.DisplayName, thingstr);

        if (item is null)
            return;

        if (!int.TryParse(amountStr, out var amount))
            ReplyToUnknownInput(aisling);

        BuyItem(aisling, item, amount);
    }
}