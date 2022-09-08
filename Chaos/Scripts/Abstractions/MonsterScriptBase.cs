using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Abstractions;

public abstract class MonsterScriptBase : ScriptBase, IMonsterScript
{
    protected Monster Monster { get; }
    protected bool ShouldMove => Monster.MoveTimer.IntervalElapsed;
    protected bool ShouldWander => Monster.WanderTimer.IntervalElapsed;
    protected bool ShouldAttack => Monster.AttackTimer.IntervalElapsed;
    protected bool ShouldCast => Monster.CastTimer.IntervalElapsed;
    protected ICollection<Skill> Skills => Monster.Skills;
    protected ICollection<Spell> Spells => Monster.Spells;

    protected Creature? Target
    {
        get => Monster.Target;
        set => Monster.Target = value;
    }
    protected MapInstance Map => Monster.MapInstance;
    protected int AggroRange => Monster.AggroRange;
    protected ConcurrentDictionary<uint, int> AggroList => Monster.AggroList;

    protected MonsterScriptBase(Monster monster) => Monster = monster;

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }

    /// <inheritdoc />
    public virtual void OnSpawn() { }

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnDeath(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, Item item) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }
}