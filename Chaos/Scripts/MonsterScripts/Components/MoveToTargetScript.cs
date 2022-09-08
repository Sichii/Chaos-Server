using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class MoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public MoveToTargetScript(Monster monster)
        : base(monster) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove || Monster.WithinRange(Target, 1))
            return;

        Monster.Pathfind(Target);
        Monster.WanderTimer.Reset();
        Monster.AttackTimer.Reset();
    }
}