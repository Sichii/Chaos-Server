using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class ThrowScript : SkillScriptBase
{
    private readonly Animation ThrowAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 123
    };

    /// <inheritdoc />
    public ThrowScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var throwDirection = context.Source.Direction;
        var thrownPoint = context.Source.DirectionalOffset(throwDirection);
        var thrownAislings = context.TargetMap.GetEntitiesAtPoint<Aisling>(thrownPoint);
        var targetPoint = thrownPoint.DirectionalOffset(throwDirection);

        //potential points are the throw point, and the 3 points around it
        var potentialTargetPoints = targetPoint.GenerateCardinalPoints()
                                               .WithConsistentDirectionBias(throwDirection)
                                               .SkipLast(1)
                                               .Prepend(targetPoint)
                                               .ToList();

        foreach (var aisling in thrownAislings)
        {
            foreach (var point in potentialTargetPoints)
                if (context.SourceMap.IsWalkable(point, CreatureType.Aisling, false))
                {
                    var aislingPoint = Point.From(aisling);
                    aisling.WarpTo(point);

                    context.SourceMap.ShowAnimation(ThrowAnimation.GetPointEffectAnimation(aislingPoint));
                    context.SourceMap.ShowAnimation(ThrowAnimation.GetPointEffectAnimation(point));

                    break;
                }
        }
    }
}