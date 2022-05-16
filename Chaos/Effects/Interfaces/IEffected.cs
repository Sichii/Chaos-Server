using Chaos.Core.Interfaces;

namespace Chaos.Effects.Interfaces;

public interface IEffected : IDeltaUpdatable
{
    IEffectsBar Effects { get; }
}