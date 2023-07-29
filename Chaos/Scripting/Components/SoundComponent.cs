using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class SoundComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ISoundComponentOptions>();
        var points = vars.GetPoints();

        if (!options.Sound.HasValue)
            return;

        context.TargetMap.PlaySound(options.Sound.Value, points.ToArray());
    }

    public interface ISoundComponentOptions
    {
        byte? Sound { get; init; }
    }
}