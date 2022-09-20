using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MapScripts.Abstractions;

public interface IMapScript : IScript
{
    void OnEntered(Creature creature);
    void OnExiting(Creature creature);
}