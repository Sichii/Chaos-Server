using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Damage;

namespace Chaos.Formulae;

public static class DamageFormulae
{
    public static readonly IDamageFormula Default = new DefaultDamageFormula();
    public static readonly IDamageFormula PureDamage = new PureDamageFormula();
}