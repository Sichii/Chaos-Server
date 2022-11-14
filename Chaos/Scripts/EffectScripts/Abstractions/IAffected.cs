using Chaos.Containers.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Abstractions;

public interface IAffected : IDeltaUpdatable
{
    IEffectsBar Effects { get; }
}