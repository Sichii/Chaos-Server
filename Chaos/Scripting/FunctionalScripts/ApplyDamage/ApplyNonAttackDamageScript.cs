using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyNonAttackDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(ApplyNonAttackDamageScript));

    public ApplyNonAttackDamageScript() => DamageFormula = DamageFormulae.PureDamage;

    public virtual void ApplyDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null
    )
    {
        damage = DamageFormula.Calculate(
            source,
            target,
            script,
            damage,
            elementOverride);

        if (damage <= 0)
            return;

        switch (target)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);

                if (!aisling.IsAlive)
                    aisling.Script.OnDeath(source);

                break;
            case Monster monster:
                monster.StatSheet.SubtractHp(damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();

                break;
            // ReSharper disable once UnusedVariable
            case Merchant merchant:
                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}