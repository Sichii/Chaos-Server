#region
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
#endregion

namespace Chaos.Formulae.AbilityUp;

public sealed class DefaultAbilityUpFormula : IAbilityUpFormula
{
    /// <inheritdoc />
    /// <remarks>
    ///     +300 is the floor value for AB1
    ///     <br />
    ///     "aisling.UserStatSheet.AbilityLevel * 10 % 7" adds some perceived randomness to the values
    /// </remarks>
    public int CalculateTna(Aisling aisling)
        => (int)(Math.Pow(aisling.UserStatSheet.AbilityLevel, 3.0d) * 10 + 300 + aisling.UserStatSheet.AbilityLevel * 10 % 7);
}