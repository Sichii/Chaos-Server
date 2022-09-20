namespace Chaos.Formulae.Abstractions;

public interface ILevelRangeFormula
{
    int GetLowerBound(int level);
    int GetUpperBound(int level);
    bool WithinLevelRange(int level1, int level2);
}