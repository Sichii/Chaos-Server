using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class AggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public AggroTargetingScript(Monster monster)
        : base(monster) => TargetUpdateTimer =
        new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(500, Monster.Template.AttackIntervalMs)));

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        TargetUpdateTimer.Update(delta);

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        if (Target is { IsAlive: false })
        {
            AggroList.Remove(Target.Id, out _);
            Monster.Target = null;
        }

        //grab the creature with the highest aggro
        //if nothing is aggro, grab the closest Aisling
        Target = Map.ObjectsVisibleTo<Creature>(Monster, AggroRange)
                                 .Where(c => c.IsAlive && (c is Aisling || AggroList.ContainsKey(c.Id)))
                                 .OrderByDescending(c => AggroList.TryGetValue(c.Id, out var aggro) ? aggro : 0)
                                 .ThenBy(c => c.DistanceFrom(Monster))
                                 .FirstOrDefault();

        if ((Target != null) && !AggroList.ContainsKey(Target.Id))
            AggroList[Target.Id] = InitialAggro++;
    }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage)
    {
        base.OnAttacked(source, damage);

        if (damage <= 0)
            return;

        AggroList.AddOrUpdate(source.Id, _ => damage, (_, currentAggro) => currentAggro + damage);
    }
}