using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

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

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.OnSameMapAs(Subject))
            return;

        Spells.Shuffle();

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