using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class WanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public WanderingScript(Monster monster)
        : base(monster) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target != null) || !ShouldWander)
            return;

        Monster.Wander();
        Monster.MoveTimer.Reset();
    }
}