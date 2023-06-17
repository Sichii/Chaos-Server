using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalItemCheckScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalItemCheckScript(Merchant subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        //only listen to aislings
        if (source is not Aisling aisling)
            return;

        //make sure only the closest banker responds
        if (!IsClosestVerbalBankerTo(aisling))
            return;

        var match = RegexCache.ITEM_CHECK_PATTERNS
                              .Select(regex => regex.Match(message))
                              .FirstOrDefault(x => x.Success);

        if (match is null)
            return;

        var thingStr = match.Groups["thing"].Value;
        var item = aisling.Bank.FuzzySearchBy(i => i.DisplayName, thingStr);

        if (item is null)
            return;

        var count = aisling.Bank.CountOf(item.DisplayName);

        Subject.Say($"{aisling.Name}, you have {count} {item.DisplayName} stashed away.");
    }
}