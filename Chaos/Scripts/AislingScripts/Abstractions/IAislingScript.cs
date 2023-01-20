using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.AislingScripts.Abstractions;

public interface IAislingScript : ICreatureScript
{
    bool CanUseItem(Item item);
    void OnClicked(Aisling source);
    void OnDeath(Creature source);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnItemDroppedOn(Aisling source, Item item);
}