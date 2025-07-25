#region
using Chaos.Models.World;
#endregion

namespace Chaos.Formulae.Abstractions;

public interface IAbilityUpFormula
{
    int CalculateTna(Aisling aisling);
}