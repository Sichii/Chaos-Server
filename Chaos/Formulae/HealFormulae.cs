using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Healing;

namespace Chaos.Formulae;

public static class HealFormulae
{
    public static IHealFormula Default { get; } = new DefaultHealFormula();
}