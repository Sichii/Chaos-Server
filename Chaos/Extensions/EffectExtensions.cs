using Chaos.Common.Definitions;
using Chaos.Scripts.EffectScripts.Abstractions;

namespace Chaos.Extensions;

public static class EffectExtensions
{
    public static EffectColor GetColor(this IEffect effect)
    {
        var remaining = effect.Remaining;

        return remaining.TotalSeconds switch
        {
            >= 60 => EffectColor.White,
            >= 30 => EffectColor.Red,
            >= 15 => EffectColor.Orange,
            >= 10 => EffectColor.Yellow,
            >= 5  => EffectColor.Green,
            > 0   => EffectColor.Blue,
            _     => EffectColor.None
        };
    }
}