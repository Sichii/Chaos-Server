using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyHealing;

public class ApplyHealScript : ScriptBase, IApplyHealScript
{
    /// <inheritdoc />
    public IHealFormula HealFormula { get; set; } = HealFormulae.Default;

    public static string Key { get; } = GetScriptKey(typeof(ApplyHealScript));

    /// <inheritdoc />
    public virtual void ApplyHeal(
        Creature source,
        Creature target,
        IScript script,
        int healing)
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