using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IHealFormula
{
    int Calculate(
        Creature source,
        Creature target,
        IScript script,
        int healing
    );
}