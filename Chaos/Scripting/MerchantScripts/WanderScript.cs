using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class WanderScript : MerchantScriptBase
{
    /// <inheritdoc />
    public WanderScript(Merchant subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Subject.WanderTimer.IntervalElapsed)
            Subject.Wander();
    }
}