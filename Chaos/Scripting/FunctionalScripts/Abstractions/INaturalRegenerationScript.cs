using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface INaturalRegenerationScript : IFunctionalScript
{
    IRegenFormula RegenFormula { get; set; }
    static virtual INaturalRegenerationScript Create() => null!;

    void Regenerate(Creature creature);
}