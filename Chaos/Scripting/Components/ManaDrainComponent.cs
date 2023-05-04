using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Components;

public class ManaDrainComponent
{
    public void ApplyManaDrain(IReadOnlyCollection<Creature> targetEntities, IManaDrainComponentOptions options)
    {
        var drain = options.ManaDrain ?? 0;

        foreach (var target in targetEntities)
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