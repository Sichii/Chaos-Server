using Chaos.Effects.Interfaces;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Factories.Interfaces;

public interface IEffectFactory
{
    IEffect CreateEffect(string effectName, Creature source, Creature target);
}