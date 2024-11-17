#region
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IExperienceDistributionScript : IFunctionalScript
{
    IExperienceFormula ExperienceFormula { get; set; }
    ILevelUpScript LevelUpScript { get; set; }
    static virtual IExperienceDistributionScript Create() => null!;

    void DistributeExperience(Creature killedCreature, params ICollection<Aisling> aislings);

    void GiveExp(Aisling aisling, long amount);
}