using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ManaCostComponent : IConditionalComponent
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaCostComponentOptions>();

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