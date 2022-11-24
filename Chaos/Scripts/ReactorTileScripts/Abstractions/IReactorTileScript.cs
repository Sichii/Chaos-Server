using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts.Abstractions;

public interface IReactorTileScript : IScript, IDeltaUpdatable
{
    void OnWalkedOn(Creature source);

    void OnGoldDroppedOn(Creature source, Money money);

    void OnItemDroppedOn(Creature source, GroundItem groundItem);

    void OnGoldPickedUpFrom(Aisling source, Money money);
    
    void OnItemPickedUpFrom(Aisling source, GroundItem groundItem);

    void OnClicked(Aisling source);
}