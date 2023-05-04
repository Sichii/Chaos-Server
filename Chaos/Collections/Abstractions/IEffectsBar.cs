using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Collections.Abstractions;

public interface IEffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    void Apply(Creature source, IEffect effect);
    bool Contains(string effectName);
    void Dispel(string effectName);
    void Terminate(string effectName);
    bool TryGetEffect(string effectName, [MaybeNullWhen(false)] out IEffect effect);
}