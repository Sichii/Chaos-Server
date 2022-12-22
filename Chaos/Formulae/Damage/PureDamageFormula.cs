using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Damage;

public class PureDamageFormula : IDamageFormula
{
    /// <inheritdoc />
    public int Calculate(Creature? attacker, Creature defender, int damage) => Convert.ToInt32(damage);
}