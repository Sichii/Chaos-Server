using System.Threading.Tasks;
using Chaos.Core.Definitions;
using Chaos.Core.Interfaces;
using Chaos.DataObjects;

namespace Chaos.Effects.Interfaces;

public interface IEffect : IDeltaUpdatable
{
    string Name { get; }
    EffectColor GetColor();
    bool ShouldSendColor();
    ValueTask<bool> TryApplyAsync(ActivationContext activationContext);
}