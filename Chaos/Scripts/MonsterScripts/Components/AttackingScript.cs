using Chaos.Core.Utilities;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class AttackingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public AttackingScript(Monster monster)
        : base(monster) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldAttack || !Monster.WithinRange(Target, 1))
            return;

        var targetDirection = Target.DirectionalRelationTo(Monster);

        if (Monster.Direction != targetDirection)
            Monster.Turn(targetDirection);

        foreach (var skill in Skills)
            if (skill.CanUse())
                if (skill.Template.IsAssail)
                    skill.Use(Monster);
                else if (Randomizer.RollChance(5))
                    skill.Use(Monster);

        Monster.WanderTimer.Reset();
        Monster.MoveTimer.Reset();
    }
}