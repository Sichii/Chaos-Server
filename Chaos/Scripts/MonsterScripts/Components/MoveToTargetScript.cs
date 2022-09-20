using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class MoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public MoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove || (Subject.DistanceFrom(Target) == 1))
            return;

        Subject.Pathfind(Target);
        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}