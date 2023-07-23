using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class DefaultBehaviorsScript : MonsterScriptBase
{
    protected virtual BlindBehavior BlindBehavior { get; }
    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultBehaviorsScript(Monster subject)
        : base(subject)
    {
        VisibilityBehavior = new VisibilityBehavior();
        RestrictionBehavior = new RestrictionBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        BlindBehavior = new BlindBehavior();
    }

    public override bool CanMove() => RestrictionBehavior.CanMove(Subject);

    public override bool CanSee(VisibleEntity entity) => VisibilityBehavior.CanSee(Subject, entity);

    public override bool CanTalk() => RestrictionBehavior.CanTalk(Subject);

    public override bool CanTurn() => RestrictionBehavior.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionBehavior.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionBehavior.CanUseSpell(Subject, spell);

    /// <inheritdoc />
    public override bool IsBlind() => BlindBehavior.IsBlind(Subject);

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);
}