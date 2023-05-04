using Chaos.Formulae.Abstractions;
using Chaos.Models.World;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface ILevelUpScript : IFunctionalScript
{
    ILevelUpFormula LevelUpFormula { get; set; }
    static virtual ILevelUpScript Create() => null!;

    void LevelUp(Aisling source);
}