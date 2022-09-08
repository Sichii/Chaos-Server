using Chaos.Time.Abstractions;

namespace Chaos.Effects.Abstractions;

public interface IEffected : IDeltaUpdatable
{
    IEffectsBar Effects { get; }
}