using Chaos.Common.Definitions;
using Chaos.Objects.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Abstractions;

public interface IEffect : IDeltaUpdatable
{
    TimeSpan Remaining { get; set; }
    EffectColor Color { get; }
    byte Icon { get; }
    string Name { get; }
    void OnApplied(Creature target);
    void OnDispelled();
    void OnReApplied(Creature target);
    void OnTerminated();
    bool ShouldApply(Creature source, Creature target);
}