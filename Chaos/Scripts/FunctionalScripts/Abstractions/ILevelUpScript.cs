using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Scripts.FunctionalScripts.Abstractions;

public interface ILevelUpScript : IFunctionalScript
{
    ILevelUpFormula LevelUpFormula { get; set; }
    static virtual ILevelUpScript Create() => null!;

    void LevelUp(Aisling source);
}