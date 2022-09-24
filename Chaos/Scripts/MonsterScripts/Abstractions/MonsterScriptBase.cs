using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Abstractions;

public abstract class MonsterScriptBase : ScriptBase, IMonsterScript
{
    protected Creature? Target
    {
        get => Subject.Target;
        set => Subject.Target = value;
    }

    protected ConcurrentDictionary<uint, int> AggroList => Subject.AggroList;
    protected int AggroRange => Subject.AggroRange;
    protected MapInstance Map => Subject.MapInstance;
    protected bool ShouldMove => Subject.MoveTimer.IntervalElapsed;
    protected bool ShouldUseSkill => Subject.SkillTimer.IntervalElapsed;
    protected bool ShouldUseSpell => Subject.SpellTimer.IntervalElapsed;
    protected bool ShouldWander => Subject.WanderTimer.IntervalElapsed;
    protected ICollection<Skill> Skills => Subject.Skills;
    protected IList<Spell> Spells => Subject.Spells;
    protected Monster Subject { get; }

    protected MonsterScriptBase(Monster subject) => Subject = subject;

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, ref int damage) { }

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