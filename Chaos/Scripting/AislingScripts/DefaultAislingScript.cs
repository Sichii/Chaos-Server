using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase
{
    private readonly IIntervalTimer SleepAnimationTimer;
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject)
        : base(subject)
    {
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
    }

    /// <inheritdoc />
    public override bool CanMove() => RestrictionBehavior.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity) => VisibilityBehavior.CanSee(Subject, entity);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionBehavior.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionBehavior.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionBehavior.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionBehavior.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionBehavior.CanUseSpell(Subject, spell);

    /// <inheritdoc />
    public override void OnDeath(Creature source)
    {
        Subject.IsDead = true;
        Subject.Refresh(true);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);

        if (SleepAnimationTimer.IntervalElapsed)
        {
            var lastManualAction = Subject.Trackers.LastManualAction;

            if (!lastManualAction.HasValue || (DateTime.UtcNow.Subtract(lastManualAction.Value).TotalMinutes > 5))
                Subject.AnimateBody(BodyAnimation.Snore);
        }
    }
}