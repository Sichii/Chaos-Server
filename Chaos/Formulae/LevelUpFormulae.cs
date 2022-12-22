using Chaos.Formulae.Abstractions;
using Chaos.Formulae.LevelUp;

namespace Chaos.Formulae;

public static class LevelUpFormulae
{
    public static ILevelUpFormula Default { get; } = new DefaultLevelUpFormula();
}