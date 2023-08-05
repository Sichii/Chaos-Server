using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;
using Chaos.Utilities;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalWithdrawItemScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalWithdrawItemScript(Merchant subject, ILogger<VerbalWithdrawItemScript> logger)
        : base(subject, logger) { }

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        //only listen to aislings
        if (source is not Aisling aisling)
            return;

        //make sure only the closest banker responds
        if (!IsClosestVerbalBankerTo(aisling))
            return;

        var match = RegexCache.WITHDRAW_PATTERNS
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

        var item = aisling.Bank.FuzzySearchBy(i => i.DisplayName, thingStr);

        if (item is null)
            return;

        if (AllTerms.Contains(amountStr))
        {
            //get the amount of the item in the bank
            var amountActual = aisling.Bank.CountOf(item.DisplayName);

            //if the item is stackable, we can only withdrawl at max enough to fill a stack
            if (item.Template.Stackable)
                amountActual = Math.Min(amountActual, item.Template.MaxStacks - aisling.Inventory.CountOf(item.DisplayName));

            WithdrawItem(aisling, amountActual, item.DisplayName);
        } else if (int.TryParse(amountStr, out var amount))
            WithdrawItem(aisling, amount, item.DisplayName);
        else
            ReplyToUnknownInput(aisling);
    }

    public virtual void WithdrawItem(Aisling source, int amount, string itemName)
    {
        var result = ComplexActionHelper.WithdrawItem(source, itemName, amount);

        switch (result)
        {
            case ComplexActionHelper.WithdrawItemResult.Success:
            {
                var phrase = WithdrawPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, itemName.ToQuantity(amount)));

                Logger.WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation(
                          "Aisling {@AislingName} withdrew {Amount} {@ItemName} from the bank",
                          source.Name,
                          amount,
                          itemName);

                break;
            }
            case ComplexActionHelper.WithdrawItemResult.CantCarry:
            {
                Subject.Say($"{source.Name}, you can't carry that many");

                break;
            }
            case ComplexActionHelper.WithdrawItemResult.DontHaveThatMany:
            {
                var phrase = DontHaveThatManyWithdrawPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, itemName.ToQuantity(amount)));

                break;
            }
            case ComplexActionHelper.WithdrawItemResult.BadInput:
            {
                ReplyToUnknownInput(source);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}