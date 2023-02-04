using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;

namespace Chaos.Scripts.Components;

public class ManaCostComponent
{
    public void ApplyManaCost(ActivationContext context, IManaCostComponentOptions options)
    {
        var cost = options.ManaCost ?? 0;
        cost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctManaCost);

        context.Source.StatSheet.SubtractMp(cost);
        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    public interface IManaCostComponentOptions
    {
        int? ManaCost { get; init; }
        decimal PctManaCost { get; init; }
    }
}