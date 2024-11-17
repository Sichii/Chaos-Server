#region
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalDepositItemScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalDepositItemScript(Merchant subject, ILogger<VerbalDepositItemScript> logger)
        : base(subject, logger) { }

    protected virtual void DepositItem(Aisling source, int amount, string itemName)
    {
        var result = ComplexActionHelper.DepositItem(source, itemName, amount);

        switch (result)
        {
            case ComplexActionHelper.DepositItemResult.Success:
            {
                RandomizedReply(
                    Subject,
                    DepositPhrases,
                    source.Name,
                    amount,
                    itemName);

                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Item,
                              Topics.Actions.Deposit
                          ])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation(
                          "Aisling {@AislingName} deposited {Amount} {@ItemName} in the bank",
                          source.Name,
                          amount,
                          itemName);

                break;
            }
            case ComplexActionHelper.DepositItemResult.DontHaveThatMany:
            {
                RandomizedReply(
                    Subject,
                    DontHaveThatManyDepositPhrases,
                    source.Name,
                    amount,
                    itemName);

                break;
            }
            case ComplexActionHelper.DepositItemResult.NotEnoughGold:
            {
                RandomizedReply(
                    Subject,
                    ItemDamagedDepositPhrases,
                    source.Name,
                    amount,
                    itemName);

                break;
            }
            case ComplexActionHelper.DepositItemResult.BadInput:
            {
                ReplyToUnknownInput(source);

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

        //make sure only the closest banker responds
        if (!IsClosestVerbalBankerTo(aisling))
            return;

        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(regex => regex.Match(message))
                              .FirstOrDefault(x => x.Success);

        if (match is null)
            return;

        var amountStr = "1";
        string thingStr;

        if (match.Groups.TryGetValue("amount", out var amountGroup) && amountGroup.Success)
            amountStr = amountGroup.Value;

        if (match.Groups.TryGetValue("thing", out var thingGroup) && thingGroup.Success)
            thingStr = thingGroup.Value;
        else
            return;

        if (MoneyTerms.Contains(thingStr))
            return;

        var item = aisling.Inventory.FuzzySearchBy(i => i.DisplayName, thingStr);

        if (item is null)
            return;

        if (AllTerms.Contains(amountStr))
        {
            //get the amount of the item in the inventory
            var amountActual = aisling.Inventory.CountOf(item.DisplayName);

            DepositItem(aisling, amountActual, item.DisplayName);
        } else if (int.TryParse(amountStr, out var amount))
            DepositItem(aisling, amount, item.DisplayName);
        else
            ReplyToUnknownInput(aisling);
    }
}