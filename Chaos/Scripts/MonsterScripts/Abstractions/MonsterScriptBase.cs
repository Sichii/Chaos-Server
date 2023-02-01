using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Abstractions;

public abstract class MonsterScriptBase : SubjectiveScriptBase<Monster>, IMonsterScript
{
    protected Creature? Target
    {
        get => Subject.Target;
        set => Subject.Target = value;
    }

    protected virtual ConcurrentDictionary<uint, int> AggroList => Subject.AggroList;
    protected virtual int AggroRange => Subject.AggroRange;
    protected virtual MapInstance Map => Subject.MapInstance;
    protected virtual bool ShouldMove => Subject.MoveTimer.IntervalElapsed;
    protected virtual bool ShouldUseSkill => Subject.SkillTimer.IntervalElapsed;
    protected virtual bool ShouldUseSpell => Subject.SpellTimer.IntervalElapsed;
    protected virtual bool ShouldWander => Subject.WanderTimer.IntervalElapsed;
    protected virtual IList<Skill> Skills => Subject.Skills;
    protected virtual IList<Spell> Spells => Subject.Spells;

    /// <inheritdoc />
    protected MonsterScriptBase(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanMove() => true;

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
    public virtual void OnAttacked(Creature attacker, int damage) => OnAttacked(attacker, damage, null);

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