using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ManaReplenishAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaReplenishComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        var replenish = options.ManaReplenish ?? 0;

        foreach (var target in targets)
        {
            var finalReplenish = replenish + MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, options.PctManaReplenish);

            target.StatSheet.AddMp(finalReplenish);
            (target as Aisling)?.Client.SendAttributes(StatUpdateType.Vitality);
        }
    }

    public interface IManaReplenishComponentOptions
    {
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
    }
}