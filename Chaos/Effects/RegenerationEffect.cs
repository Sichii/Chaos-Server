using Chaos.Effects.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Effects;

public sealed class RegenerationEffect : EffectBase
{
    public RegenerationEffect(Creature source, Creature target)
        : base(source, target) { }
}