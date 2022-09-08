using Chaos.Effects.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IEffectFactory
{
    IEffect CreateEffect(string effectName, Creature source, Creature target);
}