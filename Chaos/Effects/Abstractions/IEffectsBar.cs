using Chaos.Time.Abstractions;

namespace Chaos.Effects.Abstractions;

public interface IEffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    void Add(IEffect effect);
    void Apply(IEffect effect);
    void Dispel(string effectName);
    void Terminate(string effectName);
}