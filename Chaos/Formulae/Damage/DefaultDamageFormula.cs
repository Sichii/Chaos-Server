using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Servers.Options;

namespace Chaos.Formulae.Damage;

public class DefaultDamageFormula : IDamageFormula
{
    protected virtual int ApplyAcModifier(int ac, long damage, bool toAisling)
    {
        var minimum = toAisling ? WorldOptions.Instance.MinimumAislingAc : WorldOptions.Instance.MinimumMonsterAc;
        var maximum = toAisling ? WorldOptions.Instance.MaximumAislingAc : WorldOptions.Instance.MaximumMonsterAc;

        if (ac == 0)
            return Convert.ToInt32(damage);

        ac = Math.Clamp(ac, minimum, maximum);
        var mod = 1 + ac / 100.0m;

        return Convert.ToInt32(damage * mod);
    }

    /// <inheritdoc />
    public int Calculate(Creature? attacker, Creature attacked, long damage) =>
        ApplyAcModifier(attacked.StatSheet.Ac, damage, attacked is Aisling);
}