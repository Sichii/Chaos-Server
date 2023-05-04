using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class CastingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public CastingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;

        Spells.ShuffleInPlace();

        foreach (var spell in Spells)
            if (Subject.TryUseSpell(spell, Target.Id))
            {
                Subject.WanderTimer.Reset();
                Subject.MoveTimer.Reset();
                Subject.SkillTimer.Reset();

                break;
            }
    }
}