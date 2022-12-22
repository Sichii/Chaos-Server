using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.Abstractions;

public interface IExperienceDistributionScript
{
    void DistributeExperience(Creature killedCreature, params Aisling[] aislings);

    void GiveExp(Aisling aisling, long amount);
}