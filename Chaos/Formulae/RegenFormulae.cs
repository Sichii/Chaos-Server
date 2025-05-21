#region
using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Regen;
#endregion

namespace Chaos.Formulae;

public static class RegenFormulae
{
    public static DefaultRegenFormula Default { get; } = new();
}