#region
using Chaos.Formulae.Abstractions;
using Chaos.Formulae.LevelUp;
#endregion

namespace Chaos.Formulae;

public static class LevelUpFormulae
{
    public static DefaultLevelUpFormula Default { get; } = new();
}