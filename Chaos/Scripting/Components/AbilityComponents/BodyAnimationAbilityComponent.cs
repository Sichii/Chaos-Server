using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct BodyAnimationAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IBodyAnimationComponentOptions>();
        var animationSpeed = options.AnimationSpeed ?? 25;

        if (options.ScaleBodyAnimationSpeedByAttackSpeed is true)
        {
            var modifier = context.Source.StatSheet.EffectiveAttackSpeedPct / 100m;

            if (modifier < 0)
                animationSpeed = (ushort)(animationSpeed * Math.Abs(modifier - 1));
            else
                animationSpeed = (ushort)(animationSpeed / (1 + modifier));
        }

        context.Source.AnimateBody(options.BodyAnimation, animationSpeed);
    }

    public interface IBodyAnimationComponentOptions
    {
        ushort? AnimationSpeed { get; init; }
        BodyAnimation BodyAnimation { get; init; }
        bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    }
}