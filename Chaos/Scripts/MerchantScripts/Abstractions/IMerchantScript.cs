using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Abstractions;

public interface IMerchantScript : ICreatureScript, IScript, IDeltaUpdatable
{
    void OnApproached(Creature source);
    void OnClicked(Aisling source);
    void OnDeparture(Creature source);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnItemDroppedOn(Aisling source, byte slot, byte count);
    void OnPublicMessage(Creature source, string message);
}