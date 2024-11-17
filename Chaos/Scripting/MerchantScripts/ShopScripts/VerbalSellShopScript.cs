#region
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.ShopScripts.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.MerchantScripts.ShopScripts;

public class VerbalSellShopScript : VerbalShopScriptBase
{
    protected ISellShopSource SellShopSource { get; }

    /// <inheritdoc />
    public VerbalSellShopScript(Merchant subject, ILogger<VerbalSellShopScript> logger)
        : base(subject, logger)
    {
        if (subject is not ISellShopSource sellShopSource)
            throw new ArgumentException("Merchant must implement ISellShopSource", nameof(subject));

        SellShopSource = sellShopSource;
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

        var match = RegexCache.SELL_ITEM_PATTERNS
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

        var item = SellShopSource.ItemsToBuy.FuzzySearchBy(i => i.DisplayName, thingstr);

        if (item is null)
            return;

        if (AllTerms.ContainsI(amountStr))
            SellItem(aisling, item, aisling.Inventory.CountOf(item.DisplayName));
        else if (int.TryParse(amountStr, out var amount))
            SellItem(aisling, item, amount);
        else
            ReplyToUnknownInput(aisling);
    }

    protected virtual void SellItem(Aisling source, Item item, int amount)
    {
        var valuePerItem = item.Template.SellValue;
        var totalCost = valuePerItem * amount;

        var result = ComplexActionHelper.SellItem(
            source,
            item.DisplayName,
            amount,
            valuePerItem);

        switch (result)
        {
            case ComplexActionHelper.SellItemResult.Success:
            {
                RandomizedReply(
                    Subject,
                    SellItemPhrases,
                    source.Name,
                    amount,
                    item.DisplayName,
                    totalCost);

                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Item,
                              Topics.Entities.Gold,
                              Topics.Actions.Sell
                          ])
                      .WithProperty(source)
                      .WithProperty(item)
                      .WithProperty(SellShopSource)
                      .LogInformation(
                          "Aisling {@AislingName} sold {ItemAmount} {@ItemName} to merchant {@MerchantName} for {GoldAmount} gold",
                          source.Name,
                          amount,
                          item.DisplayName,
                          SellShopSource.Name,
                          totalCost);

                break;
            }
            case ComplexActionHelper.SellItemResult.DontHaveThatMany:
            {
                RandomizedReply(
                    Subject,
                    DontHaveThatManySellPhrases,
                    source.Name,
                    amount,
                    item.DisplayName);

                break;
            }
            case ComplexActionHelper.SellItemResult.ItemDamaged:
            {
                RandomizedReply(
                    Subject,
                    ItemDamagedSellPhrases,
                    source.Name,
                    amount,
                    item.DisplayName);

                break;
            }
            case ComplexActionHelper.SellItemResult.BadInput:
            {
                ReplyToUnknownInput(source);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}