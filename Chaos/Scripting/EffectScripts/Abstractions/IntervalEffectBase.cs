using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class IntervalEffectBase : EffectBase
{
    protected abstract IIntervalTimer Interval { get; }

    protected abstract void OnIntervalElapsed();

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Interval.Update(delta);

        if (Interval.IntervalElapsed)
            OnIntervalElapsed();
    }
}