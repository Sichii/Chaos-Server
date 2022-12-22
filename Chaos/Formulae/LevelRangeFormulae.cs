using Chaos.Formulae.Abstractions;
using Chaos.Formulae.LevelRange;

namespace Chaos.Formulae;

public static class LevelRangeFormulae
{
    public static ILevelRangeFormula Default => new DefaultLevelRangeFormula();
}