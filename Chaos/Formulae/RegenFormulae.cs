using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Regen;

namespace Chaos.Formulae;

public static class RegenFormulae
{
    public static IRegenFormula Default { get; } = new DefaultRegenFormula();
}