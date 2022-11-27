using Chaos.Common.Definitions;
using Chaos.Objects.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Abstractions;

public interface IEffect : IDeltaUpdatable
{
    EffectColor Color { get; set; }
    TimeSpan Remaining { get; set; }
    Creature Subject { get; set; }
    byte Icon { get; }
    string Name { get; }
    void OnApplied();
    void OnDispelled();
    void OnReApplied();
    void OnTerminated();
    bool ShouldApply(Creature source, Creature target);
}