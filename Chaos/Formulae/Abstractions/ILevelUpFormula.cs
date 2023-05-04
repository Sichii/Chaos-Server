using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Formulae.Abstractions;

public interface ILevelUpFormula
{
    Attributes CalculateAttributesIncrease(Aisling aisling);
    int CalculateMaxWeight(Aisling aisling);
    int CalculateTnl(Aisling aisling);
}