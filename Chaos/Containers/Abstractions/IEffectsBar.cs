using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Containers.Abstractions;

public interface IEffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    void Apply(Creature source, IEffect effect);
    bool Contains(string effectName);
    void Dispel(string effectName);
    void SimpleAdd(IEffect effect);
    void Terminate(string effectName);
    bool TryGetEffect(string effectName, [MaybeNullWhen(false)] out IEffect effect);
}

//effects are dispelled by spells (probably by name)
//effects are terminated when they expire (will have the effect object)
//effects are applied by spells (will have the effect object)
//effects are added during deserialization (will have the effect object)