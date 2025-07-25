#region
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Formulae.Experience;

public class PureExperienceFormula : IExperienceFormula
{
    /// <inheritdoc />
    public long Calculate(Creature killedCreature, params ICollection<Aisling> aislings)
        => killedCreature switch
        {
            Aisling         => 0,
            Monster monster => Convert.ToInt64(monster.Experience),
            _               => 0
        };
}