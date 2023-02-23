using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Components;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase
{
    protected virtual RestrictionComponent RestrictionComponent { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject)
        : base(subject) =>
        RestrictionComponent = new RestrictionComponent();

    /// <inheritdoc />
    public override bool CanMove() => RestrictionComponent.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionComponent.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionComponent.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionComponent.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionComponent.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionComponent.CanUseSpell(Subject, spell);

    /// <inheritdoc />
    public override void OnDeath(Creature source)
    {
        Subject.IsDead = true;
        Subject.Refresh(true);
    }
}