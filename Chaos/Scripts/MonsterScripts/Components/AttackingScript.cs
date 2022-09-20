using Chaos.Core.Utilities;
using Chaos.Geometry.Definitions;
using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.MonsterScripts.Components;

public class AttackingScript : MonsterScriptBase
{
    private ValueStopwatch? AttackDelay { get; set; }

    /// <inheritdoc />
    public AttackingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        //if target is invalid or we're not close enough
        //reset attack delay and return
        if (Target is not { IsAlive: true } || (Subject.DistanceFrom(Target) != 1))
        {
            AttackDelay = null;

            return;
        }

        var targetDirection = Target.DirectionalRelationTo(Subject);

        if (targetDirection == Direction.Invalid)
            return;

        //if attack delay is null, it means we just reached our target
        if (AttackDelay is null)
        {
            AttackDelay = ValueStopwatch.StartNew();

            return;
        }

        //we should wait a moment to turn and attack
        if (AttackDelay.Value.GetElapsedTime().TotalMilliseconds < 750)
            return;

        if (Subject.Direction != targetDirection)
            Subject.Turn(targetDirection);

        var attacked = false;

        foreach (var assail in Skills.Where(skill => skill.Template.IsAssail))
            attacked |= Subject.TryUseSkill(assail);

        if (!ShouldUseSkill)
            return;

        foreach (var skill in Skills.Where(skill => !skill.Template.IsAssail))
            if (Randomizer.RollChance(7) && Subject.TryUseSkill(skill))
            {
                attacked = true;

                break;
            }

        if (attacked)
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
        }
    }
}