using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class AggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public AggroTargetingScript(Monster subject)
        : base(subject) => TargetUpdateTimer =
        new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(500, Subject.Template.SkillIntervalMs)));

    /// <inheritdoc />
    public override void OnAttacked(Creature source, ref int damage)
    {
        base.OnAttacked(source, ref damage);
        var localDamage = damage;

        if ((damage <= 0) || source.Equals(Subject))
            return;

        AggroList.AddOrUpdate(source.Id, _ => localDamage, (_, currentAggro) => currentAggro + localDamage);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        TargetUpdateTimer.Update(delta);

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        if ((Target != null) && (!Target.IsAlive || !Target.OnSameMapAs(Subject)))
        {
            AggroList.Remove(Target.Id, out _);
            Subject.Target = null;
        }

        Target = null;

        //first try to get target via aggro list
        //if something is already aggro, ignore aggro range
        foreach (var kvp in AggroList.OrderByDescending(kvp => kvp.Value))
        {
            if (!Map.TryGetObject<Creature>(kvp.Key, out var possibleTarget))
                continue;

            if (!possibleTarget.IsAlive || !possibleTarget.IsVisibleTo(Subject) || !possibleTarget.WithinRange(Subject))
                continue;

            Target = possibleTarget;

            break;
        }

        if (Target != null)
            return;

        //if we failed to get a target via aggroList, grab the closest aisling within aggro range
        Target ??= Map.GetEntitiesWithinRange<Creature>(Subject, AggroRange)
                      .ThatAreVisibleTo(Subject)
                      .Where(obj => !obj.Equals(Subject) && obj.IsAlive)
                      .ClosestOrDefault(Subject);

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}