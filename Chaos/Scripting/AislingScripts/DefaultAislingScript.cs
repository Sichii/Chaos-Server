using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase
{
    private readonly IStore<MailBox> MailStore;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer SleepAnimationTimer;
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject, ISimpleCache simpleCache, IStore<MailBox> mailStore)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MailStore = mailStore;
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
    public override IEnumerable<BoardBase> GetBoardList()
    {
        //mailbox board
        yield return MailStore.Load(Subject.Name);
        //yield return SimpleCache.Get<BoardBase>("test_public_board");
        //yield return SimpleCache.Get<BoardBase>("test_privileged_board");
        //yield return SimpleCache.Get<BoardBase>("test_private_board");

        //things like... get board based on Nation, Guild, Enums, Flags, whatever
        //e.g.
        //var nationBoard = Subject.Nation switch
        //{
        //    Nation.Exile      => SimpleCache.Get<Board>("nation_board_exile"),
        //    Nation.Suomi      => SimpleCache.Get<Board>("nation_board_suomi"),
        //    Nation.Ellas      => SimpleCache.Get<Board>("nation_board_ellas"),
        //    Nation.Loures     => SimpleCache.Get<Board>("nation_board_loures"),
        //    Nation.Mileth     => SimpleCache.Get<Board>("nation_board_mileth"),
        //    Nation.Tagor      => SimpleCache.Get<Board>("nation_board_tagor"),
        //    Nation.Rucesion   => SimpleCache.Get<Board>("nation_board_rucesion"),
        //    Nation.Noes       => SimpleCache.Get<Board>("nation_board_noes"),
        //    Nation.Illuminati => SimpleCache.Get<Board>("nation_board_illuminati"),
        //    Nation.Piet       => SimpleCache.Get<Board>("nation_board_piet"),
        //    Nation.Atlantis   => SimpleCache.Get<Board>("nation_board_atlantis"),
        //    Nation.Abel       => SimpleCache.Get<Board>("nation_board_abel"),
        //    Nation.Undine     => SimpleCache.Get<Board>("nation_board_undine"),
        //    Nation.Purgatory  => SimpleCache.Get<Board>("nation_board_purgatory"),
        //    _                 => throw new ArgumentOutOfRangeException()
        //};
        //
        //yield return nationBoard;
    }

    /// <inheritdoc />
    public override void OnDeath()
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
            if (!Subject.IsAlive)
                return;

            var lastManualAction = Subject.Trackers.LastManualAction;

            if (!lastManualAction.HasValue || (DateTime.UtcNow.Subtract(lastManualAction.Value).TotalMinutes > 5))
                Subject.AnimateBody(BodyAnimation.Snore);
        }
    }
}