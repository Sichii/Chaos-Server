#region
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IAbilityDistributionScript : IFunctionalScript
{
    IAbilityFormula AbilityFormula { get; set; }
    IAbilityUpScript AbilityUpScript { get; set; }
    static virtual IAbilityDistributionScript Create() => null!;

    void DistributeAbility(Creature killedCreature, params ICollection<Aisling> aislings);

    void GiveAbility(Aisling aisling, long amount);
}