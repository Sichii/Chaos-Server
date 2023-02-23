using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IExperienceDistributionScript : IFunctionalScript
{
    IExperienceFormula ExperienceFormula { get; set; }
    ILevelUpScript LevelUpScript { get; set; }
    static virtual IExperienceDistributionScript Create() => null!;

    void DistributeExperience(Creature killedCreature, params Aisling[] aislings);

    void GiveExp(Aisling aisling, long amount);
}