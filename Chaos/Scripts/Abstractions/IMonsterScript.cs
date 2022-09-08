using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.Abstractions;

public interface IMonsterScript : IScript, IDeltaUpdatable
{
    void OnSpawn();
    void OnApproached(Creature source);
    void OnDeparture(Creature source);
    void OnDeath(Creature source);
    void OnAttacked(Creature source, int damage);
    void OnClicked(Aisling source);
    void OnItemDroppedOn(Aisling source, Item item);
    void OnGoldDroppedOn(Aisling source, int amount);
}