using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class BodyAnimationComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IBodyAnimationComponentOptions>();

        context.Source.AnimateBody(options.BodyAnimation, options.AnimationSpeed ?? 25);
    }

    public interface IBodyAnimationComponentOptions
    {
        ushort? AnimationSpeed { get; init; }
        BodyAnimation BodyAnimation { get; init; }
    }
}