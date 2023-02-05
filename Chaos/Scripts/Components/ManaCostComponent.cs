using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;

namespace Chaos.Scripts.Components;

public class ManaCostComponent
{
    public bool TryApplyManaCost(ActivationContext context, IManaCostComponentOptions options)
    {
        var cost = options.ManaCost ?? 0;
        cost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctManaCost);

        if (!context.Source.StatSheet.TrySubtractMp(cost))
            return false;

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);

        return true;
    }

    public interface IManaCostComponentOptions
    {
        int? ManaCost { get; init; }
        decimal PctManaCost { get; init; }
    }
}