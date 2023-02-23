using Chaos.Containers.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public interface IAffected : IDeltaUpdatable
{
    IEffectsBar Effects { get; }
}