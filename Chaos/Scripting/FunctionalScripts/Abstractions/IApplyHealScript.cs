using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IApplyHealScript : IFunctionalScript
{
    IHealFormula HealFormula { get; set; }

    void ApplyHeal(
        Creature source,
        Creature target,
        IScript script,
        int healing
    );

    static virtual IApplyHealScript Create() => throw new NotImplementedException("Override this method in your implementation");
}