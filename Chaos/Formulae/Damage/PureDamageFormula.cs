using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Damage;

public class PureDamageFormula : IDamageFormula
{
    /// <inheritdoc />
    public int Calculate(Creature? attacker, Creature attacked, int damage) => Convert.ToInt32(damage);
}