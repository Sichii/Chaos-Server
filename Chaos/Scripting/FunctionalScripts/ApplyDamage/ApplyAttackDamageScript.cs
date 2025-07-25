#region
using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; } = DamageFormulae.Default;
    public static string Key { get; } = GetScriptKey(typeof(ApplyAttackDamageScript));

    public virtual int ApplyDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null)
    {
        damage = DamageFormula.Calculate(
            source,
            target,
            script,
            damage,
            elementOverride);

        if (damage <= 0)
            return 0;

        target.Trackers.LastDamagedBy = source;

        switch (target)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                aisling.Script.OnAttacked(source, damage);

                if (!aisling.IsAlive)
                    aisling.Script.OnDeath();

                break;
            case Monster monster:
                monster.StatSheet.SubtractHp(damage);
                monster.ShowHealth();
                monster.Script.OnAttacked(source, damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();

                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(source, damage);

                break;
        }

        return damage;
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}