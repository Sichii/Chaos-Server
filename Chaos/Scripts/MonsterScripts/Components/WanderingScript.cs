using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class WanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public WanderingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target != null) || !ShouldWander)
            return;

        Subject.Wander();
        Subject.MoveTimer.Reset();
    }
}