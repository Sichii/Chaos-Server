using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalWithdrawGoldScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalWithdrawGoldScript(Merchant subject, ILogger<VerbalWithdrawGoldScript> logger)
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

        if (!MoneyTerms.Contains(thingStr))
            return;

        if (AllTerms.Contains(amountStr))
            WithdrawGold(aisling, Convert.ToInt32(aisling.Bank.Gold));
        else if (int.TryParse(amountStr, out var amount))
            WithdrawGold(aisling, amount);
        else
            ReplyToUnknownInput(aisling);
    }

    protected virtual void WithdrawGold(Aisling source, int amount)
    {
        var result = ComplexActionHelper.WithdrawGold(source, amount);

        switch (result)
        {
            case ComplexActionHelper.WithdrawGoldResult.Success:
            {
                var phrase = WithdrawPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, $"{amount} gold"));

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Actions.Withdraw)
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} withdrew {Amount} gold from the bank", source, amount);

                break;
            }
            case ComplexActionHelper.WithdrawGoldResult.TooMuchGold:
            {
                Subject.Say($"{source.Name}, you can't hold that much gold.");

                break;
            }
            case ComplexActionHelper.WithdrawGoldResult.DontHaveThatMany:
            {
                var phrase = DontHaveThatManyWithdrawPhrases.PickRandom();
                Subject.Say(phrase.Inject(source.Name, $"{amount} gold"));

                break;
            }
            case ComplexActionHelper.WithdrawGoldResult.BadInput:
            {
                ReplyToUnknownInput(source);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}