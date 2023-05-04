using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Formulae.Abstractions;

public interface IExperienceFormula
{
    long Calculate(Creature killedCreature, params Aisling[] aislings);
}