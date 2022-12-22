using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IExperienceFormula
{
    long Calculate(Creature killedCreature, params Aisling[] aislings);
}