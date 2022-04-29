using Chaos.Containers;
using Chaos.Core.Interfaces;

namespace Chaos.Effects.Interfaces;

public interface IEffected : IDeltaUpdatable
{
    EffectsBar Effects { get; init; }
}