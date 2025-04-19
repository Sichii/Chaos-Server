#region
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.EffectScripts.Abstractions;

public interface IEffect : IDeltaUpdatable, IScript
{
    EffectColor Color { get; set; }
    TimeSpan Remaining { get; set; }
    StaticVars SnapshotVars { get; set; }
    Creature Source { get; set; }
    IScript? SourceScript { get; set; }
    Creature Subject { get; set; }
    Aisling? AislingSubject { get; }
    byte Icon { get; }
    string Name { get; }
    T GetVar<T>(string key) where T: notnull;
    void OnApplied();
    void OnDispelled();
    void OnReApplied();
    void OnTerminated();
    void PrepareSnapshot(Creature source);
    void SetDuration(TimeSpan duration);
    void SetVar<T>(string key, T value) where T: notnull;
    bool ShouldApply(Creature source, Creature target);
}