using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts.BankScripts;

public class VerbalBankerScript : CompositeMerchantScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(VerbalDepositGoldScript)),
        GetScriptKey(typeof(VerbalWithdrawGoldScript)),
        GetScriptKey(typeof(VerbalGoldCheckScript)),
        GetScriptKey(typeof(VerbalDepositItemScript)),
        GetScriptKey(typeof(VerbalWithdrawItemScript)),
        GetScriptKey(typeof(VerbalItemCheckScript))
    };

    public VerbalBankerScript(IScriptProvider scriptProvider, Merchant subject)
    {
        if (scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, subject) is not CompositeMerchantScript script)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);
    }
}