using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Servers.Options;

namespace Chaos.Formulae.Damage;

public class DefaultDamageFormula : IDamageFormula
{
    private readonly ImmutableArray<ImmutableArray<decimal>> ElementalModifierLookup = new[]
    {
        // @formatter:off
        //mostly lifted from http://da-wizard.com/elements.html
        //                                                  D E F E N S E
        //                         None,  Fire,  Water,  Wind,  Earth, Holy,  Darkness,  Wood,  Metal,  Nature
        /*      None*/     new[] { 0.58m, 0.37m, 0.37m,  0.37m, 0.37m, 0.37m, 0.37m,     0.37m, 0.37m,  0.37m }.ToImmutableArray(),
        /* O    Fire*/     new[] { 2.32m, 0.58m, 0.66m,  1.74m, 1.03m, 0.93m, 0.83m,     1.74m, 0.83m,  1.88m }.ToImmutableArray(),
        /* F    Water*/    new[] { 2.32m, 1.74m, 0.58m,  1.03m, 0.66m, 0.93m, 0.83m,     0.66m, 1.88m,  0.93m }.ToImmutableArray(), 
        /* F    Wind*/     new[] { 2.32m, 0.66m, 1.03m,  0.58m, 1.74m, 0.93m, 0.83m,     1.03m, 0.50m,  0.83m }.ToImmutableArray(),
        /* E    Earth*/    new[] { 2.32m, 1.03m, 1.74m,  0.66m, 0.58m, 0.93m, 0.83m,     0.50m, 0.83m,  0.50m }.ToImmutableArray(),
        /* N    Holy*/     new[] { 2.32m, 0.76m, 0.76m,  0.76m, 0.76m, 0.58m, 1.48m,     0.50m, 0.76m,  0.76m }.ToImmutableArray(),
        /* S    Darkness*/ new[] { 2.32m, 1.25m, 1.25m,  1.25m, 1.25m, 1.48m, 0.58m,     1.48m, 1.25m,  1.25m }.ToImmutableArray(),
        /* E    Wood*/     new[] { 2.32m, 0.58m, 0.76m,  1.03m, 1.74m, 0.93m, 0.83m,     0.58m, 1.03m,  1.74m }.ToImmutableArray(),
        /*      Metal*/    new[] { 2.32m, 0.83m, 0.50m,  1.88m, 0.83m, 0.93m, 0.83m,     1.25m, 0.58m,  1.25m }.ToImmutableArray(),
        /*      Nature*/   new[] { 2.32m, 0.50m, 0.83m,  0.83m, 1.88m, 0.93m, 0.83m,     0.58m, 1.25m,  0.58m }.ToImmutableArray() 
        // @formatter:on
    }.ToImmutableArray();

    protected virtual int ApplyAcModifier(int ac, int damage, bool toAisling)
    {
        var minimum = toAisling ? WorldOptions.Instance.MinimumAislingAc : WorldOptions.Instance.MinimumMonsterAc;
        var maximum = toAisling ? WorldOptions.Instance.MaximumAislingAc : WorldOptions.Instance.MaximumMonsterAc;

        if (ac == 0)
            return Convert.ToInt32(damage);

        ac = Math.Clamp(ac, minimum, maximum);
        var mod = 1 + ac / 100.0m;

        return Convert.ToInt32(damage * mod);
    }

    protected virtual int ApplyElementalModifier(int damage, Element attackElement, Element defenseElement) =>
        Convert.ToInt32(damage * ElementalModifierLookup[(int)attackElement][(int)defenseElement]);

    /// <inheritdoc />
    public int Calculate(Creature attacker, Creature attacked, int damage)
    {
        var withAc = ApplyAcModifier(attacked.StatSheet.EffectiveAc, damage, attacked is Aisling);

        return ApplyElementalModifier(withAc, attacker.StatSheet.OffenseElement, attacked.StatSheet.DefenseElement);
    }
}