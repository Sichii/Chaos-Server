using Chaos.Common.Definitions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public interface IEffect : IDeltaUpdatable, IScript
{
    EffectColor Color { get; set; }
    TimeSpan Remaining { get; set; }
    Creature? Source { get; set; }
    Creature Subject { get; set; }
    byte Icon { get; }
    string Name { get; }
    void OnApplied();
    void OnDispelled();
    void OnReApplied();
    void OnTerminated();
    void SetDuration(TimeSpan duration);
    bool ShouldApply(Creature source, Creature target);
}