#region
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
#endregion

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IAbilityUpScript : IFunctionalScript
{
    IAbilityUpFormula AbilityUpFormula { get; set; }

    void AbilityUp(Aisling aisling);
    static virtual IAbilityUpScript Create() => null!;
}