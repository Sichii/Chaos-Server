using Chaos.Data;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Effects.Abstractions;

public abstract class AnimatingEffectBase : IntervalEffectBase
{
    protected abstract Animation Animation { get; }

    protected AnimatingEffectBase(Creature target)
        : base(target) { }

    protected override void OnIntervalElapsed() => Subject.MapInstance.ShowAnimation(Animation);
}