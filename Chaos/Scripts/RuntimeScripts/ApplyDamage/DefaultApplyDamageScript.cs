using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.RuntimeScripts.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.ApplyDamage;

public class DefaultApplyDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; }

    public DefaultApplyDamageScript(IDamageFormula damageFormula) => DamageFormula = damageFormula;

    public virtual void ApplyDamage(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    )
    {
        damage = DamageFormula.Calculate(attacker, defender, damage);

        switch (defender)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();

                if (!aisling.IsAlive)
                    PlayerDeathScripts.Default.OnDeath(aisling);

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
}