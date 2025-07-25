#region
using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Damage;
#endregion

namespace Chaos.Formulae;

public static class DamageFormulae
{
    public static readonly DefaultDamageFormula Default = new();
    public static readonly PureDamageFormula PureDamage = new();
}