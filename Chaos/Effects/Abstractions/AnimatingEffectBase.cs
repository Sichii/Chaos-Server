using Chaos.Data;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Effects.Abstractions;

public abstract class AnimatingEffectBase : IntervalEffectBase
{
    protected abstract Animation Animation { get; }

    protected AnimatingEffectBase(Creature source, Creature target)
        : base(source, target) { }

    protected override void OnIntervalElapsed() => Target.MapInstance.ShowAnimation(Animation);
}