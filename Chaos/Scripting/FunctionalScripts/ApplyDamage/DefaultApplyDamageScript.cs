using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class DefaultApplyDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(DefaultApplyDamageScript));

    public DefaultApplyDamageScript() => DamageFormula = DamageFormulae.Default;

    public virtual void ApplyDamage(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    )
    {
        damage = DamageFormula.Calculate(
            attacker,
            defender,
            source,
            damage);

        if (damage <= 0)
            return;

        switch (defender)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                aisling.Script.OnAttacked(attacker, damage);

                if (!aisling.IsAlive)
                    aisling.Script.OnDeath(attacker);

                break;
            case Monster monster:
                monster.StatSheet.SubtractHp(damage);
                monster.ShowHealth();
                monster.Script.OnAttacked(attacker, damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();

                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(attacker, damage);

                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}