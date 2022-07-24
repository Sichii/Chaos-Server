using Chaos.Core.Interfaces;
using Chaos.Time.Interfaces;

namespace Chaos.Effects.Interfaces;

public interface IEffected : IDeltaUpdatable
{
    IEffectsBar Effects { get; }
}