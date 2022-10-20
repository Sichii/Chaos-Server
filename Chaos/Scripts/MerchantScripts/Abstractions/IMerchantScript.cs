using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Abstractions;

public interface IMerchantScript : IScript, IDeltaUpdatable
{
    void OnItemDroppedOn(Aisling source, byte slot, byte count);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnApproached(Creature source);
    void OnClicked(Aisling source);
    void OnDeparture(Creature source);
    void OnAttacked(Creature source, ref int damage);
}