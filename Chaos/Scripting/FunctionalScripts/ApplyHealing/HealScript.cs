using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyHealing;

public class HealScript : ScriptBase, IApplyHealScript
{
    /// <inheritdoc />
    public IHealFormula HealFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(HealScript));

    public HealScript() => HealFormula = HealFormulae.Default;

    /// <inheritdoc />
    public void ApplyHeal(
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
                aisling.ShowHealth();
                aisling.Script.OnHealed(source, healing);

                break;
            case Monster monster:
                monster.StatSheet.AddHp(healing);
                monster.ShowHealth();
                monster.Script.OnHealed(source, healing);

                break;
            case Merchant merchant:
                merchant.Script.OnHealed(source, healing);

                break;
        }
    }

    /// <inheritdoc />
    public static IApplyHealScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyHealScript>(Key);
}