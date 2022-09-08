using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

public class CastingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public CastingScript(Monster monster)
        : base(monster) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        if (Target is not { IsAlive: true } || !ShouldCast || !Target.OnSameMapAs(Monster))
            return;

        var randomUsableSpell = Spells
                                .Where(s => s.CanUse())
                                .MinBy(_ => Random.Shared.Next());

        if (randomUsableSpell != null)
        {
            randomUsableSpell.Use(new ActivationContext(Target, Monster));
            Monster.WanderTimer.Reset();
            Monster.MoveTimer.Reset();
            Monster.AttackTimer.Reset();
        }
    }
}