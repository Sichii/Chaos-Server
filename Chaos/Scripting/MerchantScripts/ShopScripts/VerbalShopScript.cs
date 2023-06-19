using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts.ShopScripts;

public class VerbalShopScript : CompositeMerchantScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(VerbalBuyShopScript)),
        GetScriptKey(typeof(VerbalSellShopScript))
    };

    public VerbalShopScript(IScriptProvider scriptProvider, Merchant subject)
    {
        if (scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, subject) is not CompositeMerchantScript script)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);
    }
}