using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct AnimationAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IAnimationComponentOptions>();
        var points = vars.GetPoints();
        var targets = vars.GetTargets<MapEntity>();

        if (options.Animation == null)
            return;

        if (options.AnimatePoints)
            foreach (var point in points)
                context.TargetMap.ShowAnimation(options.Animation.GetPointAnimation(point, context.Source.Id));
        else
            foreach (var target in targets)
                target.Animate(options.Animation, context.Source.Id);
    }

    public interface IAnimationComponentOptions
    {
        bool AnimatePoints { get; init; }
        Animation? Animation { get; init; }
    }
}