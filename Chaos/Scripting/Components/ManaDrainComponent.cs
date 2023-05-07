using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ManaDrainComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaDrainComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        var drain = options.ManaDrain ?? 0;

        foreach (var target in targets)
        {
            var finalDrain = drain + MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, options.PctManaDrain);

            target.StatSheet.SubtractMp(finalDrain);
            (target as Aisling)?.Client.SendAttributes(StatUpdateType.Vitality);
        }
    }

    public interface IManaDrainComponentOptions
    {
        int? ManaDrain { get; init; }
        decimal PctManaDrain { get; init; }
    }
}