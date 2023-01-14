using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Damage;

public class PureDamageFormula : IDamageFormula
{
    /// <inheritdoc />
    public int Calculate(
        Creature? attacker,
        Creature defender,
        IScript source,
        int damage
    ) => Convert.ToInt32(damage);
}