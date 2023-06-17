using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalDepositItemScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalDepositItemScript(Merchant subject)
        : base(subject) { }

    protected virtual void DepositItem(
        Aisling source,
        int amount,
        string itemName
    )
    {
        var result = ComplexActionHelper.DepositItem(source, itemName, amount);

        switch (result)
        {
            case ComplexActionHelper.DepositItemResult.Success:
            {
                var phrase = DepositPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, amount, itemName));

                break;
            }
            case ComplexActionHelper.DepositItemResult.DontHaveThatMany:
            {
                var phrase = DontHaveThatManyDepositPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, amount, itemName));

                break;
            }
            case ComplexActionHelper.DepositItemResult.NotEnoughGold:
            {
                //TODO: gold fees not used in these samples

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