using Chaos.Models.World.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IRegenFormula
{
    public int CalculateHealthRegen(Creature creature);
    public int CalculateIntervalSecs(Creature creature);
    public int CalculateManaRegen(Creature creature);
}