using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IPlayerDeathScript : IFunctionalScript
{
    static virtual IPlayerDeathScript Create() => null!;

    void OnDeath(Aisling aisling, Creature killedBy);
}