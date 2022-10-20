using Chaos.Objects.Dialog;
using Chaos.Objects.World;
using Chaos.Scripts.MerchantScripts.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class DefaultMerchantScript : MerchantScriptBase
{
    /// <inheritdoc />
    public DefaultMerchantScript(Merchant subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnClicked(Aisling source) => InitialDialog?.TryActivate(source, Subject);
}