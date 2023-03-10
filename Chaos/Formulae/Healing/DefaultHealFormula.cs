using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Healing;

public class DefaultHealFormula : IHealFormula
{
    /// <inheritdoc />
    public int Calculate(
        Creature source,
        Creature target,
        IScript script,
        int healing
    ) => healing;
}