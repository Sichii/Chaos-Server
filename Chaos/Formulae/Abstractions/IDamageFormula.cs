using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IDamageFormula
{
    int Calculate(Creature attacker, Creature defender, int damage);
}