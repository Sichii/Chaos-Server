#region
using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Healing;
#endregion

namespace Chaos.Formulae;

public static class HealFormulae
{
    public static DefaultHealFormula Default { get; } = new();
}