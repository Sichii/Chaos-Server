using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IDamageFormula
{
    int Calculate(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    );
}