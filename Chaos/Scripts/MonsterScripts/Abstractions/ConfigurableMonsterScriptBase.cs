using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Abstractions;

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
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage, int? aggroOverride = null) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeath() { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, Item item) { }

    /// <inheritdoc />
    public virtual void OnSpawn() { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}