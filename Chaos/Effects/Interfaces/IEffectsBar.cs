using Chaos.Time.Interfaces;

namespace Chaos.Effects.Interfaces;

public interface IEffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    void Add(IEffect effect);
    void Apply(IEffect effect);
    void Dispel(string effectName);
    void Terminate(string effectName);
}