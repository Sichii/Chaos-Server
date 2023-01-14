using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
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

        if (!Map.GetEntitiesWithinRange<Aisling>(Subject).Any())
            return;

        Subject.Wander();
        Subject.MoveTimer.Reset();
    }
}