using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RestrictionScript : MonsterScriptBase
{
    protected virtual RestrictionBehavior RestrictionBehavior { get; }

    /// <inheritdoc />
    public RestrictionScript(Monster subject)
        : base(subject) => RestrictionBehavior = new RestrictionBehavior();

    /// <inheritdoc />
    public override bool CanMove() => RestrictionBehavior.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionBehavior.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionBehavior.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionBehavior.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionBehavior.CanUseSpell(Subject, spell);
}