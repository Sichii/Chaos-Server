using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MonsterScripts.Abstractions;

public interface IMonsterScript : ICreatureScript
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