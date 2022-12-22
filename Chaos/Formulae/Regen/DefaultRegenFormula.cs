using Chaos.Common.Utilities;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateHealthRegen(Creature creature)
    {
        var percentToRegenerate = creature switch
        {
            Aisling  => 10,
            Monster  => 3,
            Merchant => 100,
            _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
        };

        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToRegenerate);
    }

    /// <inheritdoc />
    public int CalculateIntervalSecs(Creature creature) => 6;

    /// <inheritdoc />
    public int CalculateManaRegen(Creature creature)
    {
        var percentToRegenerate = creature switch
        {
            Aisling  => 5,
            Monster  => 1.5m,
            Merchant => 100,
            _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
        };

        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, percentToRegenerate);
    }
}