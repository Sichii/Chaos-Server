#region
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Formulae.Abstractions;

public interface IExperienceFormula
{
    long Calculate(Creature killedCreature, params ICollection<Aisling> aislings);
}