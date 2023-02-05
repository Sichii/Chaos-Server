using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Abstractions;

public interface IMonsterScript : ICreatureScript, IDeltaUpdatable
{
    void OnApproached(Creature source);
    void OnAttacked(Creature source, int damage, int? aggroOverride);
    void OnClicked(Aisling source);
    void OnDeath();
    void OnDeparture(Creature source);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnItemDroppedOn(Aisling source, Item item);
    void OnSpawn();
}