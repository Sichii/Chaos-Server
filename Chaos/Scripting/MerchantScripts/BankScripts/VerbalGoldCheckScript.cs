using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public sealed class VerbalGoldCheckScript : VerbalBankerScriptBase
{
    /// <inheritdoc />
    public VerbalGoldCheckScript(Merchant subject, ILogger<VerbalGoldCheckScript> logger)
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

        if (HowMuchGoldPhrases.FuzzyContains(message, 0.75m, 0.25m))
            Subject.Say($"{aisling.Name}, you have {aisling.Bank.Gold} gold stashed away.");
    }
}