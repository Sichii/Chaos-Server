#region
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalDepositGoldScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalDepositGoldScript(Merchant subject, ILogger<VerbalDepositGoldScript> logger)
        : base(subject, logger) { }

    protected virtual void DepositGold(Aisling source, int amount)
    {
        var result = ComplexActionHelper.DepositGold(source, amount);

        switch (result)
        {
            case ComplexActionHelper.DepositGoldResult.Success:
            {
                RandomizedReply(
                    Subject,
                    DepositPhrases,
                    source.Name,
                    amount,
                    "gold");

                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Gold,
                              Topics.Actions.Deposit
                          ])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} deposited {Amount} gold in the bank", source.Name, amount);

                break;
            }
            case ComplexActionHelper.DepositGoldResult.DontHaveThatMany:
            {
                RandomizedReply(
                    Subject,
                    DontHaveThatManyDepositPhrases,
                    source.Name,
                    amount,
                    "gold");

                break;
            }
            case ComplexActionHelper.DepositGoldResult.BadInput:
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

        if (!MoneyTerms.Contains(thingStr))
            return;

        if (AllTerms.Contains(amountStr))
            DepositGold(aisling, aisling.Gold);
        else if (int.TryParse(amountStr, out var amount))
            DepositGold(aisling, amount);
        else
            ReplyToUnknownInput(aisling);
    }
}