using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public interface IReactorTileScript : IScript, IDeltaUpdatable
{
    void OnClicked(Aisling source);

    void OnGoldDroppedOn(Creature source, Money money);

    void OnGoldPickedUpFrom(Aisling source, Money money);

    void OnItemDroppedOn(Creature source, GroundItem groundItem);

    void OnItemPickedUpFrom(Aisling source, GroundItem groundItem, int originalCount);
    void OnWalkedOn(Creature source);
}