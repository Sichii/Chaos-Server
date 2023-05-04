using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Abstractions;

/// <summary>
///     This should likely only be used for quest scripts or things that are very unique. Most monster variables should be
///     interactable through the monster's properties so that they can be manipulated by buffs, debuffs, etc
/// </summary>
public abstract class ConfigurableMonsterScriptBase : ConfigurableScriptBase<Monster>, IMonsterScript
{
    /// <inheritdoc />
    protected ConfigurableMonsterScriptBase(Monster subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

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
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage, int? aggroOverride) { }

    /// <inheritdoc />
    public void OnAttacked(Creature source, int damage) => OnAttacked(source, damage, null);

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
    public virtual void OnSpawn() { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}