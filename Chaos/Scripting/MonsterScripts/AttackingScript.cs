using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class AttackingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public AttackingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //if target is invalid or we're not close enough
        //reset attack delay and return
        if (Target is not { IsAlive: true } || (Subject.DistanceFrom(Target) != 1))
            return;

        var direction = Target.DirectionalRelationTo(Subject);

        if (Subject.Direction != direction)
            return;

        var lastWalk = Subject.Trackers.LastWalk ?? DateTime.MinValue;

        if (DateTime.UtcNow.Subtract(lastWalk).TotalMilliseconds < Subject.EffectiveAssailIntervalMs)
            return;

        var attacked = false;

        foreach (var assail in Skills.Where(skill => skill.Template.IsAssail))
            attacked |= Subject.TryUseSkill(assail);

        if (ShouldUseSkill)
            foreach (var skill in Skills.Where(skill => !skill.Template.IsAssail))
                if (IntegerRandomizer.RollChance(7) && Subject.TryUseSkill(skill))
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