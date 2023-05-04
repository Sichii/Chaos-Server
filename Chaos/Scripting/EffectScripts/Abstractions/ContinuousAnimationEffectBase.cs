using Chaos.Models.Data;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class ContinuousAnimationEffectBase : IntervalEffectBase
{
    protected abstract Animation Animation { get; }
    protected abstract IIntervalTimer AnimationInterval { get; }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        AnimationInterval.Update(delta);

        if (AnimationInterval.IntervalElapsed)
            Subject.Animate(Animation);
    }
}