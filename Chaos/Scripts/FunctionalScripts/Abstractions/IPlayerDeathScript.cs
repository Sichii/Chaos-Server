using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.FunctionalScripts.Abstractions;

public interface IPlayerDeathScript : IFunctionalScript
{
    static virtual IPlayerDeathScript Create() => null!;

    void OnDeath(Aisling aisling, Creature killedBy);
}