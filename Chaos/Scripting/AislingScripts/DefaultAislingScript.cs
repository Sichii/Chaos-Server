using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Scripting.SpellScripts;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase
{
    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IIntervalTimer ClearOrangeBarTimer;
    private readonly IStore<MailBox> MailStore;
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly IEffectFactory EffectFactory;

    private SocialStatus PreAfkSocialStatus { get; set; }
    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject, IStore<MailBox> mailStore, IStore<BulletinBoard> boardStore, IEffectFactory effectFactory)
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        ClearOrangeBarTimer = new IntervalTimer(TimeSpan.FromSeconds(WorldOptions.Instance.ClearOrangeBarTimerSecs), false);
        EffectFactory = effectFactory;
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

        //change this to whatever naming scheme you want to follow for guild boards
        if (Subject.Guild is not null && BoardStore.Exists(Subject.Guild.Name))
            yield return BoardStore.Load(Subject.Guild.Name);

        yield return BoardStore.Load("public_test_board");

        //things like... get board based on Nation, Guild, Enums, Flags, whatever
        //e.g.
        //var nationBoard = Subject.Nation switch
        //{
        //    Nation.Exile      => BoardStore.Load("nation_board_exile"),
        //    Nation.Suomi      => BoardStore.Load("nation_board_suomi"),
        //    Nation.Ellas      => BoardStore.Load("nation_board_ellas"),
        //    Nation.Loures     => BoardStore.Load("nation_board_loures"),
        //    Nation.Mileth     => BoardStore.Load("nation_board_mileth"),
        //    Nation.Tagor      => BoardStore.Load("nation_board_tagor"),
        //    Nation.Rucesion   => BoardStore.Load("nation_board_rucesion"),
        //    Nation.Noes       => BoardStore.Load("nation_board_noes"),
        //    Nation.Illuminati => BoardStore.Load("nation_board_illuminati"),
        //    Nation.Piet       => BoardStore.Load("nation_board_piet"),
        //    Nation.Atlantis   => BoardStore.Load("nation_board_atlantis"),
        //    Nation.Abel       => BoardStore.Load("nation_board_abel"),
        //    Nation.Undine     => BoardStore.Load("nation_board_undine"),
        //    Nation.Purgatory  => BoardStore.Load("nation_board_purgatory"),
        //    _                 => throw new ArgumentOutOfRangeException()
        //};
        //
        //yield return nationBoard;
    }

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);
    

    public void OnUse()
        => new ComponentExecutor(null!, Subject).WithOptions(this)
            .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
            ?.Execute<ApplyEffectAbilityComponent>();
    
    /// <inheritdoc />
    public override void OnDeath() 
    {
        new ApplyNonSpellEffect(EffectFactory)
        {
            Subject = Subject,
            EffectKey = "skulled",
        }.OnApplied();
    }

    /// <inheritdoc />
    public override void OnStatIncrease(Stat stat)
    {
        if (stat == Stat.STR)
            Subject.UserStatSheet.SetMaxWeight(LevelUpFormulae.Default.CalculateMaxWeight(Subject));
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);
        ClearOrangeBarTimer.Update(delta);

        if (SleepAnimationTimer.IntervalElapsed)
        {
            var lastManualAction = Subject.Trackers.LastManualAction;

            var isAfk = !lastManualAction.HasValue
                        || (DateTime.UtcNow.Subtract(lastManualAction.Value)
                                    .TotalMinutes
                            > WorldOptions.Instance.SleepAnimationTimerMins);

            if (isAfk)
            {
                if (Subject.IsAlive)
                    Subject.AnimateBody(BodyAnimation.Snore);

                //set player to daydreaming if they are currently set to awake
                if (Subject.Options.SocialStatus != SocialStatus.DayDreaming)
                {
                    PreAfkSocialStatus = Subject.Options.SocialStatus;
                    Subject.Options.SocialStatus = SocialStatus.DayDreaming;
                }
            } else if (Subject.Options.SocialStatus == SocialStatus.DayDreaming)
                Subject.Options.SocialStatus = PreAfkSocialStatus;
        }

        if (ClearOrangeBarTimer.IntervalElapsed)
        {
            var lastOrangeBarMessage = Subject.Trackers.LastOrangeBarMessage;
            var now = DateTime.UtcNow;

            //clear if
            //an orange bar message has ever been sent
            //and the last message was sent after the last clear
            //and the time since the last message is greater than the clear timer
            var shouldClear = lastOrangeBarMessage.HasValue
                              && (lastOrangeBarMessage > (Subject.Trackers.LastOrangeBarMessageClear ?? DateTime.MinValue))
                              && (now.Subtract(lastOrangeBarMessage.Value)
                                     .TotalSeconds
                                  > WorldOptions.Instance.ClearOrangeBarTimerSecs);

            if (shouldClear)
            {
                Subject.SendServerMessage(ServerMessageType.OrangeBar1, string.Empty);
                Subject.Trackers.LastOrangeBarMessage = lastOrangeBarMessage;
                Subject.Trackers.LastOrangeBarMessageClear = now;
            }
        }
    }
}