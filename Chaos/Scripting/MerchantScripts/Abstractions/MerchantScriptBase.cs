using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Abstractions;

public abstract class MerchantScriptBase : SubjectiveScriptBase<Merchant>, IMerchantScript
{
    protected virtual MapInstance Map => Subject.MapInstance;

    /// <inheritdoc />
    protected MerchantScriptBase(Merchant subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanDropItemOn(Aisling source, Item item) => !(item.Template.AccountBound || item.Template.NoTrade);

    /// <inheritdoc />
    public virtual bool CanMove() => true;

    /// <inheritdoc />
    public virtual bool CanSee(VisibleEntity entity) => true;

    /// <inheritdoc />
    public virtual bool CanTalk() => true;

    /// <inheritdoc />
    public virtual bool CanTurn() => true;

    /// <inheritdoc />
    public virtual bool CanUseSkill(Skill skill) => true;

    /// <inheritdoc />
    public virtual bool CanUseSpell(Spell spell) => true;

    /// <inheritdoc />
    public virtual bool IsBlind() => false;

    /// <inheritdoc />
    public virtual bool IsFriendlyTo(Creature creature) => false;

    /// <inheritdoc />
    public virtual bool IsHostileTo(Creature creature) => false;

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeath() { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnHealed(Creature source, int healing) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, Item item) { }

    /// <inheritdoc />
    public virtual void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}