using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MerchantScripts.Abstractions;

public interface IMerchantScript : ICreatureScript
{
    void OnApproached(Creature source);
    void OnClicked(Aisling source);
    void OnDeparture(Creature source);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnItemDroppedOn(Aisling source, byte slot, byte count);
    void OnPublicMessage(Creature source, string message);
}