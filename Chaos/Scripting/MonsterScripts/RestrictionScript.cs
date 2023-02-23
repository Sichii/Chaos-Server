using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Components;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RestrictionScript : MonsterScriptBase
{
    protected virtual RestrictionComponent RestrictionComponent { get; }

    /// <inheritdoc />
    public RestrictionScript(Monster subject)
        : base(subject) => RestrictionComponent = new RestrictionComponent();

    /// <inheritdoc />
    public override bool CanMove() => RestrictionComponent.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionComponent.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionComponent.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionComponent.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionComponent.CanUseSpell(Subject, spell);
}