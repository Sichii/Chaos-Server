using Chaos.Scripts.EffectScripts.Abstractions;

namespace Chaos.Factories.Abstractions;

public interface IEffectFactory
{
    IEffect Create(string effectKey);
}