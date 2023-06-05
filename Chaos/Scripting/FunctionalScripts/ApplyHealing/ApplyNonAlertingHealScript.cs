using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyHealing;

public class ApplyNonAlertingHealScript : ScriptBase, IApplyHealScript
{
    /// <inheritdoc />
    public IHealFormula HealFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(ApplyNonAlertingHealScript));

    public ApplyNonAlertingHealScript() => HealFormula = HealFormulae.Default;

    /// <inheritdoc />
    public virtual void ApplyHeal(
        Creature source,
        Creature target,
        IScript script,
        int healing
    )
    {
        healing = HealFormula.Calculate(
            source,
            target,
            script,
            healing);

        if (healing <= 0)
            return;

        switch (target)
        {
            case Aisling aisling:
                aisling.StatSheet.AddHp(healing);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);

                break;
            case Monster monster:
                monster.StatSheet.AddHp(healing);

                break;
            // ReSharper disable once UnusedVariable
            case Merchant merchant:
                break;
        }
    }

    /// <inheritdoc />
    public static IApplyHealScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyHealScript>(Key);
}