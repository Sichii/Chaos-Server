using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IEffectFactory
{
    IEffect Create(string effectKey);
}