using Chaos.Core.Interfaces;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Effects.Abstractions;

public abstract class IntervalEffectBase : EffectBase
{
    protected abstract IIntervalTimer Interval { get; }

    protected IntervalEffectBase(Creature source, Creature target)
        : base(source, target) { }

    protected abstract void OnIntervalElapsed();

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Interval.IntervalElapsed)
            OnIntervalElapsed();
    }
}