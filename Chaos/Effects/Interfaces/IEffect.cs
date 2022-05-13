using Chaos.Core.Definitions;
using Chaos.Core.Interfaces;
using Chaos.Objects;

namespace Chaos.Effects.Interfaces;

public interface IEffect : IDeltaUpdatable
{
    string Name { get; }
    bool Apply(ActivationContext context);
    EffectColor GetColor();
    bool ShouldSendColor();
}